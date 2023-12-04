#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kamgam.UGUIBlurredBackground
{
    public static class EditorScheduler
    {
        static bool _registeredToEditorUpdate;

        static List<(double, Action, string)> _functionTable = new List<(double, Action, string)>();

        public static bool HasId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            foreach (var tup in _functionTable)
            {
                if (tup.Item3 == id)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Schedules the given function to be executed after delay in seconds.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="func"></param>
        /// <param name="id">If specified then existing entries with the same id are replaced by the new one.</param>
        public static void Schedule(float delay, Action func, string id = null)
        {
            registerToEditorUpdate();
            
            // Id an id was set then check if there already is a function with that id and if yes replace it.
            if (!string.IsNullOrEmpty(id))
            {
                for (int i = 0; i < _functionTable.Count; i++)
                {
                    if (_functionTable[i].Item3 == id)
                    {
                        _functionTable[i] = (EditorApplication.timeSinceStartup + delay, func, id);
                        return;
                    }
                }
            }

            _functionTable.Add((EditorApplication.timeSinceStartup + delay, func, id));
        }

        static void registerToEditorUpdate()
        {
            if (_registeredToEditorUpdate)
                return;

            EditorApplication.update += update;
            _registeredToEditorUpdate = true;
        }

        static void update()
        {
            double time = EditorApplication.timeSinceStartup;
            for (int i = _functionTable.Count-1; i >= 0; i--)
            {
                if (_functionTable[i].Item1 <= time)
                {
                    var func = _functionTable[i].Item2;
                    _functionTable.RemoveAt(i);

                    // Some shenanegans to make sure the object we are calling func on is not destroyed.
                    var behaviour = func.Target as MonoBehaviour;
                    if (func != null && func.Target != null && behaviour != null && behaviour.gameObject != null)
                    {
                        func?.Invoke();
                    }
                }
            }
        }
    }
}
#endif