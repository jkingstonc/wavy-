namespace wavynet.vm.data.items
{
    class WavyObject
    {
        public static WavyItem make_obj()
        {
            return new WavyItem(new WavyObject(), typeof(WavyObject));
        }
    }
}
