using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using UsedDrawControl = System.Windows.Forms.PictureBox;  //задать контрол, который будет исп-ся для рисования
using UsedDrawControl = System.Windows.Forms.Label;         //задать контрол, который будет исп-ся для рисования

namespace Snake {
    public enum GI { Form };
    //

    public class Snake {
      
        //ПАРАМЕТРЫ ИГРЫ ......................................................
          public GI gi = GI.Form;                     //режим граф. интерфейса
          public int MaxFruitCnt { get; set; } = 1;   //кол-во фруктов на поле (пока что больше не поддерживается)
          public int SnakeSpeed  { get; set; } = 100; //скорость змейки
          //

        //ПАРАМЕТРЫ ИГРОВОГО ПОЛЯ..............................................
          //размеры: ширина
          private int mapWidth  = 30; 
          public int MapWidth   { get => mapWidth;   set => mapWidth = value; } 
          //...высота
          private int mapHeight = 30;
          public  int MapHeight  { get => mapHeight;  set => mapHeight = value; } 
          //размер клетки
          private int brickSize = 15;         
          public int BrickSize  {
              get => brickSize; 
              set { 
                if (value < 5)
                  brickSize = 5;
                else if (value > MapWidth/10)
                  brickSize = MapWidth/10;
                else
                  brickSize = value;
              }
          }
          //

        //ПАРАМЕТРЫ ВНЕШНЕГО ВИДА..............................................
          public int ExtFieldWidth = 100;     //размер служебного поля на форме
          //цвета
			    public Color SnakeColor   { get; set; } = Color.Gray;
			    public Color SnakeEatColor{ get; set; } = Color.Gray;
			    public Color FruitColor   { get; set; } = Color.Red;
        /*--------------------------------------------------------------------*/


        //СЛУЖЕБНЫЕ ПАРАМЕТРЫ ОБЕСПЕЧЕНИЯ РАБОТОСПОСОБНОСТИ
          private Form form;
          public  Form GetForm { get => form; } //возвращает ссылку на форму
          //
          private int currMapWidth = -1;        //кол-во линий должно быть на 1 больше, чем кол-во клеток
			    private int currMapHeight = -1;
    
          private int score;              //длина змейки
			    private int fruitCnt;           //осталось фруктов на поле
			    private int snX, snY;           //позиция головы змейки
			    private int tail = 0;           //позиция хвоста
          private int dirX, dirY;         //коеф. умножения (-1, +1), задающие направление движения 
          private bool start = false;     //флаг начала игры

        //размеры элементов управления
          private int newControl_Ypos;          //позиция Y для добавления очередного элемента управления на форму
          private int newControl_Xpos;          //позиция Y для добавления очередного элемента управления на форму
          private int newCntrol_LabelWidth = 70;  //ширина подписи
          private int newCntrol_LabelHeight= 20;  //ширина подписи
          private int newCntrol_TextBoxWidth;     //ширина поля ввода (расчит-ся автомат)

      		private UsedDrawControl[] snake = new UsedDrawControl[100];     //массив элементов змейки
			    private UsedDrawControl[] Fruits = new UsedDrawControl[10];     //массив фруктов
      		private UsedDrawControl[] LinesX = new UsedDrawControl[200];    //массивы линий
    			private UsedDrawControl[] LinesY = new UsedDrawControl[200];


        //КОНСТРУКТОР
        public Snake() {
            if (gi == GI.Form) {
                form = new Form1();    //сохранить в свойстве ссылку на форму
                GenerateMap();
              //запомнить необх параметры для размещения нового элемента управления
                var btn = form.Controls["btnStart"];
                newControl_Xpos = btn.Location.X;
                newControl_Ypos = btn.Location.Y+btn.Height + 15;
                //newCntrol_LabelWidth  = btn.Width;
                newCntrol_TextBoxWidth= ExtFieldWidth-btn.Width+10;

                AddControl ("MapWidth", MapWidth);
                AddControl ("MapHeight", MapHeight);
                AddControl ("Brick size", BrickSize);
                AddControl ("Fruit cnt", MaxFruitCnt);
                AddControl ("Speed", SnakeSpeed);
            }
        }
        
        // ВСПОМОГАТЕЛЬНЫЕ ПРОЦЕДУРЫ (не влияющие на логику работы)
        //------------------------------------------------------------------------------------------------------
        /* Добавления элемента на форму */ private int AddControl(string name, int value) {
          //добавление метки
            Label lb = new Label { 
                Name = "lbl" + name.Replace(" ", ""),
                Width = newCntrol_LabelWidth,
                Height= newCntrol_LabelHeight,
                Text = name,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(newControl_Xpos, newControl_Ypos)    
            };
            form.Controls.Add(lb);
          //добавление поля ввода
            TextBox tb = new TextBox { 
                Name = "tb" + name.Replace(" ", ""),
                Width = newCntrol_TextBoxWidth,
                Height= newCntrol_LabelHeight,
                Text = value.ToString(),
                AutoSize = false,
                TextAlign =  HorizontalAlignment.Left,
                Location = new Point(newControl_Xpos + newCntrol_LabelWidth - 10, newControl_Ypos)    
            };
            form.Controls.Add(tb);
            tb.BringToFront();

            newControl_Ypos += tb.Height + 1;
            return 0;
        }
        /* Чтение значения с формы      */ private void TakeValue(string controlName, ref int parameterName) {
            if (int.TryParse(form.Controls["tb"+controlName].Text, out int tmpValue)) {
                parameterName = tmpValue;
                if (form.Controls["tb"+controlName].BackColor == Color.Tomato) 
                    form.Controls["tb"+controlName].BackColor=Control.DefaultBackColor; }
            else {
                form.Controls["tb"+controlName].BackColor=Color.Tomato; 
            }
        }
        //------------------------------------------------------------------------------------------------------

        /*-------- ГЕНЕРАЦИЯ КАРТЫ --------*/
        //режим Формы 
          private void GenerateMap() {
              ////чтение новых значений параметров с формы
              //TakeValue("MapWidth",  ref mapWidth);
              //TakeValue("MapHeight", ref mapHeight);
              //TakeValue("BrickSize", ref brickSize);

              //ПОДГОНКА МАКЕТА ФОРМЫ
				      form.Width  = (MapWidth)* BrickSize + ExtFieldWidth;
				      form.Height = (MapHeight + 1)* BrickSize + 25;
				      form.Controls["btnStart"].Left = form.Width - ExtFieldWidth + 10;
				      form.Controls["btnStart"].Width = ExtFieldWidth - 30;
				      form.Controls["lblScoreLabel"].Left = form.Width - ExtFieldWidth + 10;
				      form.Controls["lblScore"].Left = form.Controls["lblScoreLabel"].Bounds.Right;

				      /*ПРОРИСОВКА СЕТКИ*/ {
                //сперва удалить лишние горизонт и верт линии
                //  если есть лишние линии, то currMapHeight будет больше MapHeight
                //  цикл в сторону уменьшения, удаляем последние линии
				          for (int i = currMapHeight; i > MapHeight; i--) {
						        form.Controls.Remove(LinesX[i]);
						        LinesX[i] = null;
				          }
				          for (int i = currMapWidth; i > MapWidth; i--) {
						        form.Controls.Remove(LinesY[i]);
						        LinesY[i] = null;
				          }

				        //добавить недостающие горизонт линии
				          for (int i = MapHeight; i > currMapHeight; i--) {
						        UsedDrawControl fieldGrid = new UsedDrawControl {
							          BackColor = Color.Black,
							          Size = new Size(1, 1),
							          Location = new Point(0, 0)
						        };
						        form.Controls.Add(fieldGrid);
						        LinesX[i] = fieldGrid;
				          }
				        //добавить недостающие верт линии
				          for (int i = MapWidth; i > currMapWidth; i--) {
						        UsedDrawControl FieldGrid = new UsedDrawControl {
							          BackColor = Color.Black,
							          Size = new Size(1,1),
							          Location = new Point(0,0)
						        };
						        form.Controls.Add(FieldGrid);
						        LinesY[i] = FieldGrid;
				          }
				          currMapWidth = MapWidth;
				          currMapHeight = MapHeight;

				        //правим размеры горизонт линии
				          for (int i = 0; i <= currMapHeight; i++) {
						        LinesX[i].Size = new Size(currMapWidth * BrickSize, 1);
						        LinesX[i].Location = new Point(0, i * BrickSize);
				          }
				        //правим размеры вертик линии
				          for (int i = 0; i <= currMapWidth; i++) {
						        LinesY[i].Size = new Size(1, currMapHeight * BrickSize);
						        LinesY[i].Location = new Point(i * BrickSize, 0);
				          } 
              }
              
				      //form.timer.Interval = SnakeSpeed;
          }

        //Пуск змейки
        public void Start() {
				   /*/очистить старую змейку и всю карту
				   ClearObject(snake, ref Score);
				   ClearObject(Fruits, ref FruitCnt);
				   ClearObject(Trees, ref TreesCnt);
				   ClearObject(Stones, ref StoneCnt);
				   ClearObject(WaterSpots, ref WaterCnt);
				   _GenerateMap();

				   //подготовиться к старту сначала
				   lblScore.Text = "0";

				   tail = 0; Score = 0; //head = 0;
				   //dirX = 1; dirY = 0; 
				   //snX = currMapWidth / 2;
				   //snY = currMapHeight / 2;

				   //разместить все объекты на карте
				   if (AllocateElement(ElemType.Fruit, maxFruitCnt, Fruits, ref FruitCnt, fruitColor, OnePointMap) != 0) { MessageBox.Show("Не удаётся разместить фрукты на карте. Измените параметры"); return; }
				   if (AllocateElement(ElemType.Stone, maxStoneCnt, Stones, ref StoneCnt, stoneColor, OnePointMap) != 0) { MessageBox.Show("Не удаётся разместить камни на карте. Измените параметры"); return; }
				   if (AllocateElement(ElemType.Tree, maxTreesCnt, Trees, ref TreesCnt, treeColor, TreeMap) != 0) { MessageBox.Show("Не удаётся разместить деревья на карте. Измените параметры"); return; }
				   if (AllocateElement(ElemType.Water, maxWaterCnt, WaterSpots, ref WaterCnt, waterColor, WaterSpotMap) != 0) { MessageBox.Show("Не удаётся разместить лужи на карте. Измените параметры"); return; }

				   //разместить змейку
				   if (AllocateElement(ElemType.Snake, 1, snake, ref Score, snakeColor, OnePointMap) != 0) { MessageBox.Show("Не удаётся разместить змейку на карте. Измените параметры"); return; }
				   Score--;    //змейку разместили и увеличили размер массива, змейка начинаться должна с 0
				   snX = snake[0].Left / BrickSize;
				   snY = snake[0].Top / BrickSize;
				   start = true;

				   //snake[0] = new UsedDrawControl();
				   //snake[0].Size = new Size(BrickSize, BrickSize);
				   //snake[0].BackColor = snakeColor;
				   //snake[0].Location = new Point(snX * BrickSize, snY * BrickSize);
				   //Controls.Add(snake[0]);

				   //timer.Start(); */
			  }

        //Перемещение змейки

        //Проверка попадания на предмет

        //Поедание фрукта
          
        //Управление змейкой

    }




}
