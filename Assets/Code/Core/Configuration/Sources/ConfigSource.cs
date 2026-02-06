using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Core.Configuration
{
    [CreateAssetMenu(fileName = "ConfigSource", menuName = "Game/Config/Config Source")]
    public class ConfigSource : ScriptableObject
    {
        [SerializeField]
        private string _rootKey;

        [SerializeReference]
        [ValueDropdown("GetAllSections", DrawDropdownForListElements = false)]
        [ListDrawerSettings(ListElementLabelName = "Label"), HideReferenceObjectPicker]
        private IConfigSection[] _sections = Array.Empty<IConfigSection>();

        public string RootKey => _rootKey;
        public IReadOnlyList<IConfigSection> Sections => _sections;

#if UNITY_EDITOR
        private IEnumerable GetAllSections()
        {
            return ConfigSourceHelper.GetAllSections();
        }
#endif
    }
}