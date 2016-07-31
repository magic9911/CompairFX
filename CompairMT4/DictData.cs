using System.Collections.Generic;

namespace TextOrder {
    public class DictData : Dictionary<string, string> {
        public void Add(string id,string positions) {
            base.Add(id, positions);
        }
    }
}
