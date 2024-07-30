using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelLoader : MonoBehaviour
{
    [SerializeField] List<string> _ModelsKey = new();
    [SerializeField] List<GameObject> _ModelsValue = new();

    Dictionary<string, GameObject> _models;

    GameObject _currentLoadedModel;

    // Start is called before the first frame update
    void Awake()
    {
        _models = new Dictionary<string, GameObject>();
        for (int i = 0; i < _ModelsKey.Count; i++)
        {
            if (i > _ModelsValue.Count)
            {
                throw new System.IndexOutOfRangeException("[PlayerModelLoader] There's more keys than values!");
            }
            print($"Key: {_ModelsKey[i]} - Value: {_ModelsValue[i].name}");
            _models.Add(_ModelsKey[i], _ModelsValue[i]);
        }
    }

    public void LoadModel()
    {
        if (_currentLoadedModel != null)
            return;
        _currentLoadedModel = GameObject.Instantiate(_models[Idealist._instance._name], GI._PlayerFetcher().transform, false);
    }

    public Animator GetModelAnimator()
    {
        if (_currentLoadedModel == null) // Quite a stoopid fix innit m8?
            LoadModel();

        Animator direct = _currentLoadedModel.GetComponent<Animator>();
        Animator inChild = null;

        Animator[] animatorsInChildren = _currentLoadedModel.GetComponentsInChildren<Animator>();
        if (animatorsInChildren.Length > 0)
        {
            inChild = animatorsInChildren[0];
        }

        if (direct == null && inChild == null)
        {
            Debug.LogWarning("[PlayerModelLoader] No Animator component found on the model or its children.");
        }

        return direct != null ? direct : inChild;
    }
}
