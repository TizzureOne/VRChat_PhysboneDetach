using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Dynamics.PhysBone.Components;

public class CheckAndMoveVRCPhysBones : Editor
{
    // 当用户右键点击游戏对象时，检查是否可以执行移植操作
    [MenuItem("GameObject/动骨移植", true, 0)]
    static bool TransferCheck()
    {
        if (Selection.objects.Length > 0)
        {
            return true; // 如果选择了至少一个游戏对象，则允许移植操作
        }
        else
        {
            return false; // 如果没有选择游戏对象，则禁用移植操作
        }
    }

    // 当用户点击菜单项 "GameObject/动骨移植" 时，执行实际的移植操作
    [MenuItem("GameObject/动骨移植", false, 0)]
    public static void TransferPhysBones()
    {
        // 创建一个新的游戏对象，用于存储选定游戏对象的动态骨骼
        GameObject transferRoot = new GameObject("PB_Group_" + Selection.activeGameObject.name);
        transferRoot.transform.SetParent(Selection.activeGameObject.transform);

        // 获取选定游戏对象及其所有子对象的变换组件
        Transform[] allchild = Selection.activeGameObject.GetComponentsInChildren<Transform>();

        // 遍历所有子对象的变换组件
        foreach (Transform child in allchild)
        {
            // 检查子对象是否具有动态骨骼或碰撞器组件
            bool bone = child.TryGetComponent(out VRCPhysBone physBone);
            bool collider = child.TryGetComponent(out VRCPhysBoneCollider physBoneCollider);
            
            // 如果子对象既没有动态骨骼组件也没有碰撞器组件，则跳过
            if (!bone && !collider) continue;

            // 创建一个新的空游戏对象，用于存储复制的组件
            GameObject container = new GameObject(child.name);
            container.transform.SetParent(transferRoot.transform);

            // 如果子对象具有动态骨骼组件，则执行以下操作
            if (bone)
            {
                // 如果动态骨骼的根变换为空，则将其设置为自身的变换
                if (physBone.rootTransform == null) physBone.rootTransform = physBone.gameObject.transform;
                
                // 复制并粘贴动态骨骼组件到新的容器对象
                UnityEditorInternal.ComponentUtility.CopyComponent(physBone);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(container);
                
                // 删除原始的动态骨骼组件
                DestroyImmediate(physBone);
            }

            // 如果子对象具有碰撞器组件，则执行以下操作
            if (collider)
            {
                // 如果碰撞器的根变换为空，则将其设置为自身的变换
                if (physBoneCollider.rootTransform == null) physBoneCollider.rootTransform = physBoneCollider.gameObject.transform;
                
                // 复制并粘贴碰撞器组件到新的容器对象
                UnityEditorInternal.ComponentUtility.CopyComponent(physBoneCollider);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(container);
                
                // 删除原始的碰撞器组件
                DestroyImmediate(physBoneCollider);
            }
        }
    }
}
