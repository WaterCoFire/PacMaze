using System;
using UnityEngine;

namespace Test {
    public class GhostronAnimationTest : MonoBehaviour {
        private Animator _animator;

        private void Start() {
            _animator = GetComponent<Animator>();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.C)) {
                _animator.SetBool("Open_Anim", false);
                _animator.SetBool("Walk_Anim", false);
            }

            if (Input.GetKeyDown(KeyCode.O)) {
                _animator.SetBool("Open_Anim", true);
                _animator.SetBool("Walk_Anim", true);
            }

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("anim_close")) {
                if (stateInfo.normalizedTime >= 1f) {
                    Debug.Log("anim_close FINISHED!!");
                }
            }

            // if (Input.GetKeyDown(KeyCode.K)) {
            //     if (scaredMaterial == null || normalMaterial == null) {
            //         Debug.LogWarning("Please set normal and scared material");
            //         return;
            //     }
            //
            //     MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
            //     int count = 0, changeCount = 0;
            //
            //     foreach (MeshRenderer renderer in renderers) {
            //         Material[] originalMats = renderer.sharedMaterials;
            //         bool hasChange = false;
            //         Material[] newMats = new Material[originalMats.Length];
            //
            //         for (int i = 0; i < originalMats.Length; i++) {
            //             if (originalMats[i] == normalMaterial) {
            //                 newMats[i] = scaredMaterial;
            //                 hasChange = true;
            //                 changeCount++;
            //             } else {
            //                 newMats[i] = originalMats[i]; // Keep original material
            //             }
            //         }
            //
            //         if (hasChange) {
            //             renderer.sharedMaterials = newMats;
            //             count++;
            //         }
            //     }
            //
            // }
            //
            // if (Input.GetKeyDown(KeyCode.L)) {
            //     if (scaredMaterial == null || normalMaterial == null) {
            //         Debug.LogWarning("Please set normal and scared material");
            //         return;
            //     }
            //
            //     MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
            //     int count = 0, changeCount = 0;
            //
            //     foreach (MeshRenderer renderer in renderers) {
            //         Material[] originalMats = renderer.sharedMaterials;
            //         bool hasChange = false;
            //         Material[] newMats = new Material[originalMats.Length];
            //
            //         for (int i = 0; i < originalMats.Length; i++) {
            //             if (originalMats[i] == scaredMaterial) {
            //                 newMats[i] = normalMaterial;
            //                 hasChange = true;
            //                 changeCount++;
            //             } else {
            //                 newMats[i] = originalMats[i]; // Keep original material
            //             }
            //         }
            //
            //         if (hasChange) {
            //             renderer.sharedMaterials = newMats;
            //             count++;
            //         }
            //     }
            // }
        }
    }
}