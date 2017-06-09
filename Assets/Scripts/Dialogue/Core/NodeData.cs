﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [Serializable]
    public enum NodeType { LINE, CHOICE }

    [Serializable]
    public class NodeData : MonoBehaviour {
        [SerializeField]
        private NodeType type;
        public NodeType Type {
            get { return type; }
        }
        
        [SerializeField]
        private GameObject speaker;
        public GameObject Speaker {
            get { return speaker; }
            set { speaker = value; }
        }

        [SerializeField]
        private string text;
        public string Text {
            get { return text; }
            set { text = value; }
        }

        [SerializeField]
        private Condition condition;
        public Condition Condition {
            get { return condition; }
            set { condition = value; }
        }
        
        [SerializeField]
        private UnityEvent action;
        public UnityEvent Action {
            get { return action; }
            set { action = value; }
        }

        [SerializeField]
        private string notes;
        public string Notes {
            get { return notes; }
            set { notes = value; }
        }

        public void Init(NodeType type, Transform parentObject) {
            this.type = type;
            Text = "Add text here";
            Condition = gameObject.AddComponent<Condition>();
            gameObject.transform.SetParent(parentObject);
            gameObject.name = "dialogue_nodedata";
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        } 
    }
}