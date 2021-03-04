using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleCalculator
{
    public class MainPageViewModel : BaseNotify
    {

        int currentState = 1;
        string mathOperator;
        double firstNumber, secondNumber;
        Random rand = new Random();
        string resultText;
        public string ResultText { get => resultText; set => RaiseAndUpdate(ref resultText, value); }

        string currentCalculation;
        public string CurrentCalculation { get => currentCalculation; set => RaiseAndUpdate(ref currentCalculation, value); }

        private List<CalcButton> buttons;
        private bool goingCrazy = false;

        public List<CalcButton> Buttons
        { get => buttons; set => RaiseAndUpdate(ref buttons, value); }

        public List<Position> Positions { get; set; }

        public ICommand ButtonCommand { get; set; }

        public bool GoingCrazy { get => goingCrazy; set => RaiseAndUpdate(ref goingCrazy, value); }

        public MainPageViewModel()
        {
            OnClear(null);

            CreateButtonsAndPositions();
        }

        async Task ChangePositions()
        {
            await Task.Delay(1000);

            foreach (var position in Positions)
            {
                position.Assigned = false;
            }

            foreach (var button in buttons)
            {

                var count = Positions.Count(p => p.Assigned == false);
                var index = rand.Next(0, count - 1);
                var position = Positions.Where(p => p.Assigned == false).ToArray()[index];
                position.Assigned = true;
                button.Position = position;
            }
        }

        private void CreateButtonsAndPositions()
        {
            Positions = new List<Position>();

            ButtonCommand = new Command<CalcButton>(OnButtonCommand);

            for (int r = 4; r > -1; r--)
            {
                for (int c = 0; c < 4; c++)
                {
                    Positions.Add(new Position
                    {
                        Row = r,
                        Column = c
                    });
                }
            }


            Buttons = new List<CalcButton> { new CalcButton
                {
                    Text = "00",
                    Position = Positions[0],
                    Action = OnClear

                },
                new CalcButton
                {
                    Text = "0",
                    Position = Positions[1],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = ".",
                    Position = Positions[2],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "=",
                    Position = Positions[3],
                    Action = OnCalculate
                },
                new CalcButton
                {
                    Text = "1",
                    Position = Positions[4],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "2",
                    Position = Positions[5],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "3",
                    Position = Positions[6],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "+",
                    Position = Positions[7],
                    Action = OnSelectOperator
                },
                new CalcButton
                {
                    Text = "4",
                    Position = Positions[8],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "5",
                    Position = Positions[9],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "6",
                    Position = Positions[10],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "-",
                    Position = Positions[11],
                    Action = OnSelectOperator
                },
                new CalcButton
                {
                    Text = "7",
                    Position = Positions[12],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "8",
                    Position = Positions[13],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "9",
                    Position = Positions[14],
                    Action = OnSelectNumber
                },
                new CalcButton
                {
                    Text = "×",
                    Position = Positions[15],
                    Action = OnSelectOperator
                },
                new CalcButton
                {
                    Text = " ",
                    Position = Positions[16]
                },
                new CalcButton
                {
                    Text = "+/-",
                    Position = Positions[17],
                    Action = OnNegative
                },
                new CalcButton
                {
                    Text = "%",
                    Position = Positions[18],
                    Action = OnPercentage
                },
                new CalcButton
                {
                    Text = "÷",
                    Position = Positions[19],
                    Action = OnSelectOperator
                }};
        }

        void OnButtonCommand(CalcButton button)
        {
            button.Action(button.Text);
        }

        void OnSelectNumber(string param)
        {
            string pressed = param;

            if (this.ResultText == "0" || currentState < 0)
            {
                this.ResultText = "";
                if (currentState < 0)
                    currentState *= -1;
            }

            if (pressed == ".")
                pressed = ".00";// not optimistic

            this.ResultText += pressed;

            double number;
            if (double.TryParse(this.ResultText, out number))
            {
                this.ResultText = number.ToString("N0");
                if (currentState == 1)
                {
                    firstNumber = number;
                }
                else
                {
                    secondNumber = number;
                }
            }
        }

        void OnSelectOperator(string param)
        {
            currentState = -2;
            string pressed = param;
            mathOperator = pressed;
        }

        void OnClear(string param)
        {
            firstNumber = 0;
            secondNumber = 0;
            currentState = 1;
            this.ResultText = "0";
        }

        async void OnCalculate(string param)
        {
            if (currentState == 2)
            {
                double result = Calculator.Calculate(firstNumber, secondNumber, mathOperator);

                this.CurrentCalculation = $"{firstNumber} {mathOperator} {secondNumber}";

                this.ResultText = result.ToTrimmedString();
                firstNumber = result;
                currentState = -1;
            }

            GoingCrazy = true;

            await ChangePositions();

            GoingCrazy = false;
        }



        void OnNegative(string param)
        {
            if (currentState == 1)
            {
                secondNumber = -1;
                mathOperator = "×";
                currentState = 2;
                OnCalculate(null);
            }
        }

        void OnPercentage(string param)
        {
            if (currentState == 1)
            {
                secondNumber = 0.01;
                mathOperator = "×";
                currentState = 2;
                OnCalculate(null);
            }

        }


    }


    public class CalcButton : BaseNotify
    {
        private Position position;

        public string Text { get; set; }
        public Position Position { get => position; set => RaiseAndUpdate(ref position, value); }
        public Action<string> Action { get; set; }
    }

    public class Position
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool Assigned { get; set; }
    }
}


public class BaseNotify : INotifyPropertyChanged
{
    protected bool RaiseAndUpdate<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        Raise(propertyName);

        return true;
    }

    protected void Raise(string propertyName)
    {
        if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
