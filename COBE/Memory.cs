using System;

namespace COBE {

    [Serializable]
    public class Memory {

        private string mnemonic;
        private long address;
        private long size;

        public string Mnemonic {
            get { return mnemonic; }
        }

        public long Address {
            get { return address; }
        }

        public long Size {
            get { return size; }
        }

        public Memory(string mnemonic, long address, long size) {
            this.mnemonic = mnemonic;
            this.address = address;
            this.size = size;
        }
    }
}
