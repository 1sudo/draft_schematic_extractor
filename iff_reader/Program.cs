namespace iff_reader
{
    class Program
    {
        static void Main(string[] args)
        {
            Reader reader = new("shared_sword_ryyk_blade.iff");

            reader.Read();
        }
    }
}
