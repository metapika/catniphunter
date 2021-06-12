﻿using UnityEngine;
using UnityCore.Menu;
namespace UnityCore {
    
    namespace Scene {
        public class TestScene : MonoBehaviour
        {
            public SceneController sceneController;

#region Unity Function
#if UNITY_EDITOR
            private void Update() {
                if(Input.GetKeyUp(KeyCode.M)) {
                    sceneController.Load(SceneType.Menus, (_scene) => {
                        Debug.Log("Scene [" + _scene + "] loaded from test script!" );
                    },false, PageType.Loading);
                }

                if(Input.GetKeyUp(KeyCode.G)) {
                    sceneController.Load(SceneType.TutorialLevel);
                }
            }
#endif
#endregion
        }
    }
}
