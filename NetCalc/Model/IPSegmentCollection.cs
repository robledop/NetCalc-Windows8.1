using System;
using System.Collections;
using System.Collections.Generic;


namespace NetCalc
{
    public class IPSegmentCollection : IEnumerable<IPSegment>, IEnumerator<IPSegment>
    {
        private double _enumerator;
        private byte _cidrSubnet;
        private IPSegment _ipnetwork;

        private byte _cidr
        {
            get { return this._ipnetwork.CIDR; }
        }

        private string _mask
        {
            get { return this._ipnetwork.SubnetMask; }
        }
        private uint _broadcast
        {
            get { return this._ipnetwork.BroadcastAddress; }
        }
        private uint _network
        {
            get { return this._ipnetwork.NetworkAddress; }
        }

        public IPSegmentCollection(IPSegment ipnetwork, byte cidrSubnet)
        {

            if (cidrSubnet > 32)
            {
                throw new ArgumentOutOfRangeException("cidrSubnet");
            }

            if (cidrSubnet < ipnetwork.CIDR)
            {
                throw new ArgumentException("cidr");
            }

            this._cidrSubnet = cidrSubnet;
            this._ipnetwork = ipnetwork;
            this._enumerator = -1;
        }

        #region Count, Array, Enumerator

        public double Count
        {
            get
            {
                double count = Math.Pow(2, this._cidrSubnet - this._cidr);
                return count;
            }
        }

        public IPSegment this[double i ]
        {
            get
            {
                if (i - 1 >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("i");
                }
                double size = this.Count;
                int increment = (int)((this._broadcast - this._network) / size);
                uint uintNetwork = (uint)(this._network + ((increment + 1) * i));
                IPSegment ipn = new IPSegment(uintNetwork.ToIpString(), this._cidrSubnet);
                return ipn;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator<IPSegment> IEnumerable<IPSegment>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #region IEnumerator<IPNetwork> Members

        public IPSegment Current
        {
            get { return this[this._enumerator]; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // nothing to dispose
            return;
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            // Por questões de performance só os primeiros 65536 itens são retornados
            this._enumerator++;
            if (this._enumerator >= this.Count || this._enumerator >= 65536)
            {
                //throw new Exception("Limit reached");
                return false;
            }
            return true;

        }

        public void Reset()
        {
            this._enumerator = -1;
        }

        #endregion

        #endregion

    }
}
