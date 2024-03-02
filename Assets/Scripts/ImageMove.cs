using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMove : MonoBehaviour
{
    [SerializeField] GameObject maskGameObject;
    [SerializeField] GameObject leftMaskGameObject;
    [SerializeField] GameObject rightMaskGameObject;
    [SerializeField] GameObject mainMenuPanel;

    public void OnCrackComplete()
    {
        maskGameObject.SetActive(false);
        leftMaskGameObject.SetActive(true);
        rightMaskGameObject.SetActive(true);
        mainMenuPanel.SetActive(true);
    }

    public void OnCombineComplete()
    {
        SceneController sceneController = FindObjectOfType<SceneController>();
        if(sceneController.IsAboutGameShouldBeActive)
        {
            sceneController.AboutGame();
        }

        if(sceneController.IsCharacterSelectionShouldBeActive)
        {
            sceneController.CharacterSelection();
        }
        if(sceneController.IsLoadGameShouldBeActive)
        {
            sceneController.LoadGame();
        }
    }
}
