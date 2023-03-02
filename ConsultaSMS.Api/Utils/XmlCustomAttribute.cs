using System;
using WebConsultaSMS.Models.Enums;

namespace WebConsultaSMS.Utils
{
    public class XmlCustomAttribute : Attribute
    {
        private int position;
        private PadAccordingToType padDirections;
        private int fillNumber;
        private char fillWith;

        public XmlCustomAttribute(
            int position,
            PadAccordingToType padDirections,
            int fillNumber,
            char fillWith
        )
        {
            this.position = position;
            this.padDirections = padDirections;
            this.fillNumber = fillNumber;
            this.fillWith = fillWith;
        }

        public virtual int Position
        {
            get { return position; }
        }
        public virtual PadAccordingToType PadDirections
        {
            get { return padDirections; }
        }

        public virtual int FillNumber
        {
            get { return fillNumber; }
        }
        public virtual char FillWith
        {
            get { return fillWith; }
        }

    }
}
