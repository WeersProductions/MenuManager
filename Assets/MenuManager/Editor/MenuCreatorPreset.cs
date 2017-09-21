using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeersProductions
{
    public class MenuCreatorPreset : ScriptableObject
    {
        [SerializeField] private string _title;

        [SerializeField] private string _description;

        [SerializeField] private GameObject _presetObject;

        public string Title
        {
            get { return _title; }
        }

        public string Description
        {
            get { return _description; }
        }

        public GameObject PresetObject
        {
            get { return _presetObject; }
        }
    }
}