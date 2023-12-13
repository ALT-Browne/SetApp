namespace SetApp.Models
{
        public class Game
        {
                public List<List<Card>> DeckHistory { get; set; }
                public List<Board> BoardHistory { get; set; }
                public List<List<Tuple<Card, Card, Card>>> SetsFoundHistory { get; set; }
                public List<Card> CurrentDeck { get; set; } = new List<Card>();
                public Board CurrentBoard { get; set; } = new Board(new List<Card>());
                public List<Tuple<Card, Card, Card>> CurrentSetsFound { get; set; } = new List<Tuple<Card, Card, Card>>();
                private int _currentMove;
                public int CurrentMove
                {
                        get
                        {
                                return _currentMove;
                        }
                        set
                        {
                                _currentMove = value;
                                if (!FixedGame)
                                {
                                        CurrentDeck = new List<Card>();
                                        DeckHistory[value].ForEach(card => CurrentDeck.Add(new Card(card)));

                                        CurrentBoard = new Board(BoardHistory[value]);
                                }

                                CurrentSetsFound = new List<Tuple<Card, Card, Card>>();
                                SetsFoundHistory[value].ForEach(tuple => CurrentSetsFound.Add(Tuple.Create(new Card(tuple.Item1), new Card(tuple.Item2), new Card(tuple.Item3))));
                        }

                }
                public bool FixedGame { get; set; }
                public string CheckSetMessage { get; set; } = "";
                public string HintMessage { get; set; } = "";
                public string DealMoreCardsMessage { get; set; } = "";

                public void GiveHint(bool fixedGame)
                {
                        HintMessage = "";
                        DealMoreCardsMessage = "";
                        CheckSetMessage = "";
                        CurrentBoard.Cards.ForEach(card => card.Selected = false);
                        if (!fixedGame)
                        {
                                if (CurrentBoard.Sets.Count == 0)
                                {
                                        HintMessage = "There are no sets.";
                                }
                                else
                                {
                                        var random = new Random();
                                        int randomIndex = random.Next(CurrentBoard.Sets.Count);

                                        CurrentBoard.Cards.Where(card => card.ImageString == CurrentBoard.Sets[randomIndex].Item1.ImageString).First().Selected = true;
                                        CurrentBoard.Cards.Where(card => card.ImageString == CurrentBoard.Sets[randomIndex].Item2.ImageString).First().Selected = true;
                                        HintMessage = "Find the third card that makes a set!";
                                }
                        }
                        else
                        {
                                if (CurrentBoard.Sets.Count == CurrentSetsFound.Count)
                                {
                                        HintMessage = "There are no sets.";
                                }
                                else
                                {
                                        List<Tuple<Card, Card, Card>> setsNotFound = CurrentBoard.Sets.Where(tuple => !CurrentSetsFound.Any(setFound => new HashSet<string>() { setFound.Item1.ImageString, setFound.Item2.ImageString, setFound.Item3.ImageString }.SetEquals(new List<string>() { tuple.Item1.ImageString, tuple.Item2.ImageString, tuple.Item3.ImageString }))).ToList();

                                        var random = new Random();
                                        int randomIndex = random.Next(setsNotFound.Count);

                                        CurrentBoard.Cards.Where(card => card.ImageString == setsNotFound[randomIndex].Item1.ImageString).First().Selected = true;
                                        CurrentBoard.Cards.Where(card => card.ImageString == setsNotFound[randomIndex].Item2.ImageString).First().Selected = true;
                                        HintMessage = "Find the third card that makes a set!";
                                }
                        }

                }

                public bool SetFound(int[] indices)
                {
                        return CurrentBoard.Sets.Any(tuple => new HashSet<string>() {tuple.Item1.ImageString, tuple.Item2.ImageString,
tuple.Item3.ImageString}.SetEquals(new List<string>() {CurrentBoard.Cards[indices[0]].ImageString, CurrentBoard.Cards[indices[1]].ImageString,
CurrentBoard.Cards[indices[2]].ImageString}));
                }

                public bool FixedGameSetFound(int[] indices)
                {
                        return CurrentBoard.Sets.Any(tuple => new HashSet<string>() {tuple.Item1.ImageString, tuple.Item2.ImageString,
tuple.Item3.ImageString}.SetEquals(new List<string>() {CurrentBoard.Cards[indices[0]].ImageString, CurrentBoard.Cards[indices[1]].ImageString,
CurrentBoard.Cards[indices[2]].ImageString})) && !CurrentSetsFound.Any(tuple => new HashSet<string>() {tuple.Item1.ImageString, tuple.Item2.ImageString,
tuple.Item3.ImageString}.SetEquals(new List<string>() {CurrentBoard.Cards[indices[0]].ImageString, CurrentBoard.Cards[indices[1]].ImageString,
CurrentBoard.Cards[indices[2]].ImageString}));
                }

                public void CheckSet(bool fixedGame)
                {
                        HintMessage = "";
                        DealMoreCardsMessage = "";
                        CheckSetMessage = "";

                        int[] indices = CurrentBoard.Cards.Select((card, index) => new { card, index }).Where(obj => obj.card.Selected == true).Select(obj => obj.index).ToArray();
                        if (indices.Length == 3)
                        {
                                CurrentBoard.Cards.ForEach(card => card.Selected = false);
                                if (!fixedGame)
                                {
                                        if (SetFound(indices))
                                        {
                                                CheckSetMessage = "You found a set!";
                                                RemoveSetCards(indices, false);
                                        }
                                        else
                                        {
                                                CheckSetMessage = "No set.";
                                        }
                                }
                                else
                                {
                                        if (FixedGameSetFound(indices))
                                        {
                                                CheckSetMessage = "You found a set!";
                                                RemoveSetCards(indices, true);
                                        }
                                        else
                                        {
                                                CheckSetMessage = "No set.";
                                        }
                                }

                        }
                        else
                        {
                                CheckSetMessage = "Not enought cards to make a set.";
                        }

                }

                public void RemoveSetCards(int[] indices, bool fixedGame)
                {
                        var nextBoardHistory = new List<Board>();
                        for (int i = 0; i <= CurrentMove; i++)
                        {
                                nextBoardHistory.Add(new Board(BoardHistory[i]));
                        }

                        var nextSetsFoundHistory = new List<List<Tuple<Card, Card, Card>>>();
                        for (int i = 0; i <= CurrentMove; i++)
                        {
                                var newList = new List<Tuple<Card, Card, Card>>();
                                for (int j = 0; j < SetsFoundHistory[i].Count; j++)
                                {
                                        newList.Add(Tuple.Create(new Card(SetsFoundHistory[i][j].Item1), new Card(SetsFoundHistory[i][j].Item2), new Card(SetsFoundHistory[i][j].Item3)));
                                }
                                nextSetsFoundHistory.Add(newList);
                        }

                        var nextDeckHistory = new List<List<Card>>();
                        for (int i = 0; i <= CurrentMove; i++)
                        {
                                var newList = new List<Card>();
                                for (int j = 0; j < DeckHistory[i].Count; j++)
                                {
                                        newList.Add(new Card(DeckHistory[i][j]));
                                }
                                nextDeckHistory.Add(newList);
                        }

                        var nextBoard = new Board(CurrentBoard);
                        var nextSetsFound = new List<Tuple<Card, Card, Card>>();

                        CurrentSetsFound.ForEach(tuple => nextSetsFound.Add(Tuple.Create(new Card(tuple.Item1), new Card(tuple.Item2), new Card(tuple.Item3))));

                        var nextDeck = new List<Card>();
                        CurrentDeck.ForEach(card => nextDeck.Add(new Card(card)));

                        nextSetsFound.Add(Tuple.Create(new Card(CurrentBoard.Cards[indices[0]]), new Card(CurrentBoard.Cards[indices[1]]), new Card(CurrentBoard.Cards[indices[2]])));

                        if (!fixedGame)
                        {
                                if (nextBoard.Cards.Count <= 12 && nextDeck.Count >= 3)
                                {
                                        for (int i = 0; i <= 2; i++)
                                        {
                                                nextBoard.Cards[indices[i]] = nextDeck[nextDeck.Count - 1];
                                                nextDeck.RemoveAt(nextDeck.Count - 1);
                                        }
                                }
                                else
                                {
                                        nextBoard.Cards.RemoveAll(card => indices.Contains(nextBoard.Cards.IndexOf(card)));
                                }
                        }

                        nextBoard.Sets = nextBoard.GetSets(nextBoard.Cards);
                        nextBoardHistory.Add(nextBoard);
                        BoardHistory.Clear();
                        nextBoardHistory.ForEach(board => BoardHistory.Add(new Board(board)));

                        nextSetsFoundHistory.Add(nextSetsFound);
                        SetsFoundHistory.Clear();
                        nextSetsFoundHistory.ForEach(list =>
                        {
                                SetsFoundHistory.Add(list.Select(tuple => Tuple.Create(new Card(tuple.Item1), new Card(tuple.Item2), new Card(tuple.Item3))).ToList());
                        });

                        Random random = new Random();
                        nextDeck = nextDeck.OrderBy(x => random.Next()).ToList();
                        nextDeckHistory.Add(nextDeck);
                        DeckHistory.Clear();
                        for (int i = 0; i < nextDeckHistory.Count; i++)
                        {
                                var newList = new List<Card>();
                                for (int j = 0; j < nextDeckHistory[i].Count; j++)
                                {
                                        newList.Add(new Card(nextDeckHistory[i][j]));
                                }
                                DeckHistory.Add(newList);
                        }

                        CurrentMove = nextBoardHistory.Count - 1;
                }

                public void AddCards(int numCards)
                {
                        if (numCards <= CurrentDeck.Count)
                        {
                                var nextBoardHistory = new List<Board>();
                                for (int i = 0; i <= CurrentMove; i++)
                                {
                                        nextBoardHistory.Add(new Board(BoardHistory[i]));
                                }

                                var nextSetsFoundHistory = new List<List<Tuple<Card, Card, Card>>>();
                                for (int i = 0; i <= CurrentMove; i++)
                                {
                                        var newList = new List<Tuple<Card, Card, Card>>();
                                        for (int j = 0; j < SetsFoundHistory[i].Count; j++)
                                        {
                                                newList.Add(Tuple.Create(new Card(SetsFoundHistory[i][j].Item1), new Card(SetsFoundHistory[i][j].Item2), new Card(SetsFoundHistory[i][j].Item3)));
                                        }
                                        nextSetsFoundHistory.Add(newList);
                                }

                                var nextDeckHistory = new List<List<Card>>();
                                for (int i = 0; i <= CurrentMove; i++)
                                {
                                        var newList = new List<Card>();
                                        for (int j = 0; j < DeckHistory[i].Count; j++)
                                        {
                                                newList.Add(new Card(DeckHistory[i][j]));
                                        }
                                        nextDeckHistory.Add(newList);
                                }

                                var nextBoard = new Board(CurrentBoard);
                                var nextSetsFound = new List<Tuple<Card, Card, Card>>();

                                CurrentSetsFound.ForEach(tuple => nextSetsFound.Add(Tuple.Create(new Card(tuple.Item1), new Card(tuple.Item2), new Card(tuple.Item3))));

                                var nextDeck = new List<Card>();
                                CurrentDeck.ForEach(card => nextDeck.Add(new Card(card)));

                                for (int i = 0; i < numCards; i++)
                                {
                                        nextBoard.Cards.Add(nextDeck[nextDeck.Count - 1]);
                                        nextDeck.RemoveAt(nextDeck.Count - 1);
                                }

                                nextBoard.Sets = nextBoard.GetSets(nextBoard.Cards);
                                nextBoardHistory.Add(nextBoard);
                                BoardHistory.Clear();
                                nextBoardHistory.ForEach(board => BoardHistory.Add(new Board(board)));

                                nextSetsFoundHistory.Add(nextSetsFound);
                                SetsFoundHistory.Clear();
                                nextSetsFoundHistory.ForEach(list =>
                                {
                                        SetsFoundHistory.Add(list.Select(tuple => Tuple.Create(new Card(tuple.Item1), new Card(tuple.Item2), new Card(tuple.Item3))).ToList());
                                });

                                Random random = new Random();
                                nextDeck = nextDeck.OrderBy(x => random.Next()).ToList();
                                nextDeckHistory.Add(nextDeck);
                                DeckHistory.Clear();
                                for (int i = 0; i < nextDeckHistory.Count; i++)
                                {
                                        var newList = new List<Card>();
                                        for (int j = 0; j < nextDeckHistory[i].Count; j++)
                                        {
                                                newList.Add(new Card(nextDeckHistory[i][j]));
                                        }
                                        DeckHistory.Add(newList);
                                }

                                CurrentMove = nextBoardHistory.Count - 1;
                        }
                }

                static Dictionary<string, string[]> characteristicStringMap = new(){
                        {"Number", new string[] {"one", "two", "three"}},
                        {"Colour", new string[] {"red", "purple", "green"}},
                        {"Shading", new string[] {"solid", "striped", "open"}},
                        {"Shape", new string[] {"squiggle", "diamond", "oval"}}
                };

                private IEnumerable<Card> allSetCards = from number in new int[] { 0, 1, 2 }
                                                        from colour in new int[] { 0, 1, 2 }
                                                        from shading in new int[] { 0, 1, 2 }
                                                        from shape in new int[] { 0, 1, 2 }
                                                        select new Card([number, colour, shading, shape], characteristicStringMap["Number"][number] + "-" +
                                                                characteristicStringMap["Colour"][colour] + "-" + characteristicStringMap["Shading"][shading] + "-" +
                                                                characteristicStringMap["Shape"][shape]);

                public Game(bool fixedGame)
                {
                        FixedGame = fixedGame;
                        Random random = new Random();
                        List<Card> newRandomDeck = allSetCards.OrderBy(x => random.Next()).ToList();
                        List<Card> firstCards = [];
                        if (!FixedGame)
                        {
                                for (int i = 0; i < 12; i++)
                                {
                                        firstCards.Add(newRandomDeck[newRandomDeck.Count - 1]);
                                        newRandomDeck.RemoveAt(newRandomDeck.Count - 1);
                                }
                        }
                        DeckHistory = new List<List<Card>>() { newRandomDeck };
                        BoardHistory = new List<Board>() { new Board(firstCards) };
                        SetsFoundHistory = new List<List<Tuple<Card, Card, Card>>>() { new List<Tuple<Card, Card, Card>>() };
                        CurrentMove = 0;
                }
        }
}
