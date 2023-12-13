namespace SetApp.Models
{
        public class Board
        {
                public List<Card> Cards { get; set; }
                public List<Tuple<Card, Card, Card>> Sets { get; set; }
                public Board(List<Card> cards)
                {
                        Cards = cards;
                        Sets = GetSets(cards);
                }

                /*Copy constructor*/
                public Board(Board prevBoard)
                {
                        Cards = new List<Card>();
                        prevBoard.Cards.ForEach(card => Cards.Add(new Card(card)));
                        Sets = new List<Tuple<Card, Card, Card>>();
                        prevBoard.Sets.ForEach(tuple => Sets.Add(Tuple.Create(new Card(tuple.Item1), new Card(tuple.Item2), new Card(tuple.Item3))));
                }

                public List<Tuple<Card, Card, Card>> GetSets(List<Card> cards)
                {
                        var sets = new List<Tuple<Card, Card, Card>>();
                        int numCards = cards.Count();
                        for (int i = 0; i < numCards; i++)
                        {
                                for (int j = i + 1; j < numCards; j++)
                                {
                                        for (int k = j + 1; k < numCards; k++)
                                        {
                                                if (IsSet(cards[i], cards[j], cards[k]))
                                                {
                                                        sets.Add(new Tuple<Card, Card, Card>(cards[i], cards[j], cards[k]));
                                                }
                                        }
                                }
                        }
                        return sets;
                }

                public bool IsSet(Card card1, Card card2, Card card3)
                {
                        return Enumerable.Zip(card1.Characteristics, card2.Characteristics,
                        card3.Characteristics).All(tuple => (tuple.Item1 + tuple.Item2 + tuple.Item3) % 3 == 0);
                }
        }
}
