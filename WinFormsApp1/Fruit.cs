using System.Drawing;
using System.Windows.Forms;

namespace Snake {              
    using static Snake;
    public class Fruit : AElement {

        //конструктор
        internal Fruit(ElementId id, string labelName, int maxCount, int maxArrayLenght, Color color) : base (id, labelName, maxCount, maxArrayLenght, color) {
          //задать координаты фруктов вручную при тестировании
            #if TEST
              testPoints = new[,] { {2, 0}, {4,0}, {6,0}, {9,0}, {13,0}, {16,0} };
            #endif
        }

        //расположение фрукта на карте
        public override bool AllocateElement() {
            #if TEST
            //если режим тестирования,задать точки вручную
              CustomPoint = new Point (testPoints[testPt_i,0], testPoints[testPt_i++,1]); 
              if (testPt_i == 6) testPt_i=0;
            #endif

            return base.AllocateElement();
        }

        public override void HeadOver() {
            #if TEST 
            //в режиме тестирования отобразить номера перекрытых элементов
              //отобразить номер элемента массива snakeArr
                bricksArr[0].Text = score.ToString();
                bricksArr[0].Font = new Font ("Arial", 6);
                bricksArr[0].BringToFront();

              //cоздать метку с индексом элемента, который перекрыт
                Label tmpLabel = new Label {
                    Location = new Point (snakeArr[score].Location.X + snakeArr[tail].Size.Width-testLabelSize.Width, snakeArr[score].Location.Y + snakeArr[tail].Size.Height - testLabelSize.Height),
                    Size = testLabelSize,
                    Text = score.ToString(),
                    BackColor = SnakeColor,
                    Font = new Font("Arial", 6)
                };
                overlapedIDDict.Add(score, tmpLabel);
                form.Controls.Add(tmpLabel);
            #endif

			    //разместить на карте новый фрукт 
				    AllocateElement(); 
        }

         public override void AddControl() {}
    }
}
