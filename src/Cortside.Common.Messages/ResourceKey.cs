using System;

namespace Cortside.Common.Messages {
    /// <summary>
    /// Summary description for ResourceKey.
    /// </summary>
    public class ResourceKey {
        private string context;
        private string field;
        private int identity;

        public string Context {
            get { return context; }
            set { context = value; }
        }

        public string Field {
            get { return field; }
            set { field = value; }
        }

        public int Identity {
            get { return identity; }
            set { identity = value; }
        }

        public ResourceKey(string newContext, string newField, int newIdentity) {
            context = newContext;
            field = newField;
            identity = newIdentity;
        }

        public ResourceKey(string newContext, string newField) {
            context = newContext;
            field = newField;
        }

    }
}
