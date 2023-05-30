
using System;
using System.Collections.Generic;

namespace NetCalc
{
    public class IPSegment : IComparable<IPSegment>
    {
        // IP e máscara em formato uint
        private uint _ip;
        private uint _mask;

        public IPSegment(string ip, string mask)
        {
            _ip = ip.ParseIp();
            _mask = mask.ParseIp();
        }

        public IPSegment(string ip, byte cidr)
        {
            _ip = ip.ParseIp();
            _mask = CIDRToMask(cidr);
        }

        public bool RFC3021
        {
            get 
            {
                if (this.CIDR == 31)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private byte MaskToCIDR(UInt32 mask)
        {
            UInt32 __mask = mask;
            __mask = __mask - ((__mask >> 1) & 0x55555555);
            __mask = (__mask & 0x33333333) + ((__mask >> 2) & 0x33333333);
            return Convert.ToByte((((__mask + (__mask >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24);
        }

        private uint CIDRToMask(byte cidr)
        {
            uint mask = 0xFFFFFFFF;
            mask = mask << (32 - cidr);
            return mask;
        }

        public byte CIDR
        {
            get 
            {
                return MaskToCIDR(_mask);
            }
        }

        public string SubnetMask
        {
            get
            {
                return _mask.ToIpString();
            }
        }

        public uint NumberOfHosts
        {
            get 
            {
                uint allIPs = ~_mask + 1;

                uint hosts = 0;

                if (allIPs > 2)
                {
                    hosts = allIPs - 2;
                }
                else if (allIPs == 2 && CIDR == 31)
                {
                    hosts = 2;
                }
                else if (allIPs == 1)
                {
                    hosts = 1;
                }

                return hosts;
            }
        }

        public uint NetworkAddress
        {
            get { return this._ip & this._mask; }
        }

        public uint BroadcastAddress
        {
            get { return NetworkAddress + ~_mask; }
        }

        public IEnumerable<uint> Hosts()
        {
            for (var host = NetworkAddress + 1; host < BroadcastAddress; host++)
            {
                yield return host;
            }
        }

        /// <summary>
        /// First usable IP adress in Network
        /// </summary>
        public uint FirstUsable
        {
            get
            {
                if (this.CIDR == 31 || this.CIDR == 32)
                {
                    return this.NetworkAddress;
                }
                else
                {
                    return (this.Usable <= 0) ? this.NetworkAddress : this.NetworkAddress + 1;
                }
            }
        }

        /// <summary>
        /// Last usable IP adress in Network
        /// </summary>
        public uint LastUsable
        {
            get
            {
                if (this.CIDR == 31 || this.CIDR == 32)
                {
                    return this.BroadcastAddress;
                }
                else
                {
                    return (this.Usable <= 0) ? this.NetworkAddress : this.BroadcastAddress - 1;
                }
            }
        }

        public string WildCardSubnetMask
        {
            get
            {
                return (~_mask).ToIpString();
            }
        }
        /// <summary>
        /// Number of usable IP adress in Network
        /// </summary>
        public uint Usable
        {
            get
            {
                //return (this.CIDR > 32) ? 0 : ((0xffffffff >> this.CIDR) - 1);
                return ((0xffffffff >> this.CIDR) - 1);
            }
        }

        public int CompareTo(IPSegment other)
        {
            int network = this.NetworkAddress.CompareTo(other.NetworkAddress);
            if (network != 0)
            {
                return network;
            }

            int cidr = this.CIDR.CompareTo(other.CIDR);
            return cidr;
        }
    }
}