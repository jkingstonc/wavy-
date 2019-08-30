namespace wavynet.vm.data.items
{
    class WavyObject : WavyItem
    {
        public WavyObject() : base(null, ItemType.OBJECT)
        {

        }

        public static WavyItem make_obj()
        {
            return new WavyItem(new WavyObject(), ItemType.OBJECT);
        }
    }
}
