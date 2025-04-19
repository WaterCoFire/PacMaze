using System;
using Entity.Ghostron;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Test {
    public class GhostronTest : MonoBehaviour {
        public GameObject pacman;
        private Animator _animator;

        public float animatorSpeed;

        private void Start() {
            _animator = GetComponent<Animator>();
            
            // _animator.SetBool("Walk_Anim", false);
            // _animator.SetBool("Open_Anim", true);
            
            GetComponent<NavMeshAgent>().SetDestination(pacman.transform.position);
        }

        private void Update() {
            // AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            // Debug.Log(
            //     $"当前状态名Hash: {stateInfo.fullPathHash}, 进度: {stateInfo.normalizedTime:F2}, 是否循环: {stateInfo.loop}");
            //
            // if (stateInfo.IsName("anim_Walk_Loop")) {
            //     Debug.Log("当前是 Walk 状态");
            // }
            
            _animator.speed = animatorSpeed;
        }
    }
}