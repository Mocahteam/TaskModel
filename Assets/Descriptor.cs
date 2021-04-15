using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Descriptor : MonoBehaviour
{
    private static Transform getDeepChild (Transform transform, string name)
    {
        Transform child = transform.Find(name);
        if (child != null)
            return child;
        for (int i = 0; i < transform.childCount; i++)
        {
            child = getDeepChild(transform.GetChild(i), name);
            if (child != null)
                return child;
        }
        return null;
    }

    private static void setUpState(GameObject gameObject, bool state)
    {
        Transform transform = getDeepChild(gameObject.transform, "Up");
        if (transform != null)
            transform.GetComponent<Button>().interactable = state;
    }

    private static void setDownState(GameObject gameObject, bool state)
    {
        Transform transform = getDeepChild(gameObject.transform, "Down");
        if (transform != null)
            transform.GetComponent<Button>().interactable = state;
    }

    private static GameObject getPreviousDescriptor (GameObject gameObject)
    {
        if (gameObject.transform.GetSiblingIndex() < 3)
            return null;
        else
            return gameObject.transform.parent.GetChild(gameObject.transform.GetSiblingIndex() - 1).gameObject;
    }

    private static GameObject getNextDescriptor(GameObject gameObject)
    {
        if (gameObject.transform.GetSiblingIndex() < gameObject.transform.parent.childCount - 1)
            return gameObject.transform.parent.GetChild(gameObject.transform.GetSiblingIndex() + 1).gameObject; 
        else
            return null;
    }

    private static void updateState(GameObject gameObject)
    {
        // Par défaut on active les deux boutons
        setUpState(gameObject, true);
        setDownState(gameObject, true);
        // si l'élément créé est le 3ème enfant (<=> premier déplacable)
        if (gameObject.transform.GetSiblingIndex() == 2)
            // désactiver le bouton Up
            setUpState(gameObject, false);
        // si l'élément créé est le dernier enfant (<=> dernier déplacable)
        if (gameObject.transform.GetSiblingIndex() == gameObject.transform.parent.childCount-1)
            // désactiver le bouton Down
            setDownState(gameObject, false);
    }

    private void updateStateAndNeighbours()
    {
        updateState(gameObject);
        GameObject nextDescriptor = getNextDescriptor(gameObject);
        if (nextDescriptor)
            updateState(nextDescriptor);
        GameObject previousDescriptor = getPreviousDescriptor(gameObject);
        if (previousDescriptor)
            updateState(previousDescriptor);
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.transform.parent as RectTransform);
    }

    // Start is called before the first frame update
    protected void Start()
    {
        updateStateAndNeighbours();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveUp()
    {
        gameObject.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex()-1);
        updateStateAndNeighbours();
    }

    public void moveDown()
    {
        gameObject.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);
        updateStateAndNeighbours();
    }

    public void copyDescriptor()
    {
        GameObject copy = Instantiate(gameObject);
        copy.transform.SetParent(gameObject.transform.parent);
        copy.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);
    }

    public void removeDescriptor()
    {
        GameObject nextDescriptor = getNextDescriptor(gameObject);
        GameObject previousDescriptor = getPreviousDescriptor(gameObject);
        // Vérifier si on retire le premier descripteur déplacable et qu'il y a un suivant
        if (gameObject.transform.GetSiblingIndex() == 2 && nextDescriptor != null)
            setUpState(nextDescriptor, false);
        // Vérifier si on retire le dernier descripteur déplacable et qu'il y a un précédent
        if (gameObject.transform.GetSiblingIndex() == gameObject.transform.parent.childCount - 1 && previousDescriptor != null)
            setDownState(previousDescriptor, false);
        Destroy(gameObject);
    }
}
