using FYFY;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        if (gameObject.transform.GetSiblingIndex() - 1 < 1)
            return null;
        else
        {
            GameObject previousGO = gameObject.transform.parent.GetChild(gameObject.transform.GetSiblingIndex() - 1).gameObject;
            if (getDeepChild(previousGO.transform, "Down") == null) // if previous Go doesn't contain Down button
                return null;
            else
                return gameObject.transform.parent.GetChild(gameObject.transform.GetSiblingIndex() - 1).gameObject;
        }
    }

    private static GameObject getNextDescriptor(GameObject gameObject)
    {
        if (gameObject.transform.GetSiblingIndex() < gameObject.transform.parent.childCount - 1)
            return gameObject.transform.parent.GetChild(gameObject.transform.GetSiblingIndex() + 1).gameObject; 
        else
            return null;
    }

    private static void updateMyMovingStates(GameObject gameObject)
    {
        // Par défaut on active les deux boutons
        setUpState(gameObject, true);
        setDownState(gameObject, true);
        // si l'élément précédent ne contient pas de bouton "Down"
        GameObject previousDescriptor = getPreviousDescriptor(gameObject);
        if (previousDescriptor == null || getDeepChild(previousDescriptor.transform, "Down") == null)
            // désactiver le bouton Up
            setUpState(gameObject, false);
        // si l'élément créé est le dernier enfant (<=> dernier déplacable)
        if (gameObject.transform.GetSiblingIndex() == gameObject.transform.parent.childCount-1)
            // désactiver le bouton Down
            setDownState(gameObject, false);
    }

    private void updateMyAndNeighboursMovingStates()
    {
        updateMyMovingStates(gameObject);
        GameObject nextDescriptor = getNextDescriptor(gameObject);
        if (nextDescriptor)
            updateMyMovingStates(nextDescriptor);
        GameObject previousDescriptor = getPreviousDescriptor(gameObject);
        if (previousDescriptor)
            updateMyMovingStates(previousDescriptor);
    }

    // Start is called before the first frame update
    protected void Start()
    {
        updateMyAndNeighboursMovingStates();
        if (gameObject.GetComponentInChildren<TMP_InputField>() != null)
            EventSystem.current.SetSelectedGameObject(gameObject.GetComponentInChildren<TMP_InputField>().gameObject);
        if (gameObject.transform.GetSiblingIndex() == gameObject.transform.parent.childCount - 1)
            StartCoroutine(MoveDown());
    }

    private IEnumerator MoveDown()
    {
        // wait two frames
        yield return null;
        yield return null;
        if (gameObject.transform.parent.parent.parent.GetComponentInChildren<Scrollbar>() != null)
            gameObject.transform.parent.parent.parent.GetComponentInChildren<Scrollbar>().value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator movingAnimation(int step)
    {
        while (gameObject.GetComponent<Animation>().IsPlaying("RemoveDescriptor"))
            yield return null;
        gameObject.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + step);
        updateMyAndNeighboursMovingStates();
        gameObject.GetComponent<Animation>().Play("InsertDescriptor");
    }

    public void moveUp()
    {
        gameObject.GetComponent<Animation>().Play("RemoveDescriptor");
        StartCoroutine(movingAnimation(-1));
    }

    public void moveDown()
    {
        gameObject.GetComponent<Animation>().Play("RemoveDescriptor");
        StartCoroutine(movingAnimation(1));
    }

    public void copyDescriptor()
    {
        GameObject copy = Instantiate(gameObject);
        copy.transform.SetParent(gameObject.transform.parent); // not using GameObjectManager.setGameObjectParent because we can't immediate set sibling
        copy.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);
        GameObjectManager.bind(copy);
        GameObjectManager.refresh(gameObject.transform.parent.gameObject); // but we ask Fyfy to synchronise parent to take into account the new child
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
        GameObjectManager.unbind(gameObject);
        gameObject.GetComponent<Animation>().Play("RemoveDescriptor");
        StartCoroutine(destroyDescriptor());
    }

    // overrided in Participant and Decision
    public virtual void resizeContainer()
    {
        
    }

    private IEnumerator destroyDescriptor()
    {
        while (gameObject.GetComponent<Animation>().IsPlaying("RemoveDescriptor"))
            yield return null;
        resizeContainer();
        Destroy(gameObject);
    }

    public void toggleDescriptor(bool state)
    {
        int step = state ? -1 : 1;
        RectTransform contentArea = transform.Find("Content") as RectTransform;
        if (contentArea)
        {
            GameObjectManager.setGameObjectState(contentArea.gameObject, !state);
            RectTransform descriptorArea = transform as RectTransform;
            descriptorArea.sizeDelta = new Vector2(descriptorArea.rect.width, descriptorArea.rect.height + step*contentArea.rect.height);
        }
    }
}
