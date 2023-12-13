namespace SetApp.Models
{
        public class Card
        {
                public int[] Characteristics { get; set; } /*(Number, Colour, Shading, Shape)*/
                public bool Selected { get; set; } = false;
                public string ImageString { get; set; } /*"two-red-solid-diamond"*/

                public Card(int[] characteristics, string imageString)
                {
                        Characteristics = characteristics;
                        ImageString = imageString;
                }

                /*Copy constructor*/
                public Card(Card prevCard)
                {
                        Characteristics = (int[])prevCard.Characteristics.Clone();
                        Selected = prevCard.Selected;
                        ImageString = prevCard.ImageString;
                }
        }
}
