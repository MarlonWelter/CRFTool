
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public class LWQuboidPlace : IQuboidPlaceLogic, IHas<IQuboidPlaceLogic>
    {
        private Guid mitId;
        public Guid GWId
        {
            get { return mitId; }
            set { mitId = value; }
        }
        public double Back { get; set; }
        public double Down { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }

        public double Right
        {
            get { return Left + Width; }
        }
        public double Front
        {
            get { return Back + Length; }
        }
        public double Up
        {
            get { return Down + Height; }
        }
        public void Fill(double left, double back, double down, double width, double length, double height)
        {
            Left = left;
            Back = back;
            Down = down;
            Width = width;
            Length = length;
            Height = height;
        }
        public LWQuboidPlace()
        {
        }
        public LWQuboidPlace(double left, double back, double down, double width, double length, double height)
        {
            this.Fill(left, back, down, width, length, height);
        }
        public LWQuboidPlace(IHas<IQuboidPlaceLogic> model)
        {
            //this.TakeSimpleValues<IQuboidPlaceLogic>(model);
            Left = model.Left();
            Width = model.Width();
            Back = model.Back();
            Length = model.Length();
            Down = model.Down();
            Height = model.Height();
        }

       
        public IQuboidPlaceLogic Logic
        {
            get { return this; }
        }
    }
}
