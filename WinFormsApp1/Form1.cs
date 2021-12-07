using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using UsedDrawControl = System.Windows.Forms.PictureBox;  //задать контрол, который будет исп-ся для рисования
using UsedDrawControl = System.Windows.Forms.Label;         //задать контрол, который будет исп-ся для рисования

namespace Snake {
	 public partial class Form1 : Form {
	 
			  public int maxFruitCnt = 1;		//пока что больше не поддерживается
			public int maxStoneCnt;
			public int maxTreesCnt;
			public int maxWaterCnt;

			  public Color snakeColor = Color.Gray;
			  public Color snakeEatColor = Color.Gray;
			  public Color fruitColor = Color.Red;
			public Color treeColor = Color.Green;
			public Color stoneColor = Color.Black;
			public Color waterColor = Color.Blue;

			  public int BrickSize;
			  public int currMapWidth = -1;   //кол-во линий должно быть на 1 больше, чем кол-во клеток
			  public int currMapHeight = -1;
			
			  public int ExtFieldWidth = 100;
			  public int SnakeSpeed;

			  private int Score;          //длина змейки
			  private int FruitCnt ;
			private int StoneCnt ;
			private int TreesCnt ;
			private int WaterCnt ;

			  private UsedDrawControl[] snakeBodie = new UsedDrawControl[100];
			  private UsedDrawControl[] Fruits = new UsedDrawControl[10];
			private UsedDrawControl[] Stones = new UsedDrawControl[60];
			private UsedDrawControl[] WaterSpots = new UsedDrawControl[200];
			private UsedDrawControl[] Trees = new UsedDrawControl[200];
			  private UsedDrawControl[] LinesX = new UsedDrawControl[200];
			  private UsedDrawControl[] LinesY = new UsedDrawControl[200];

			  private bool startFlag = false;
			  private int dirX, dirY;         //коеф. умножения (-1, +1), задающие направление движения 
			  private int snX, snY;           //позиция головы змейки
			  private int tail = 0;           //позиция хвоста
			//private int head = 0;           //позиция головы
			//private class UsedDrawControl : System.Windows.Forms.PictureBox { }       //задать контрол, который будет исп-ся для рисования (вар.2)
			//private class UsedDrawControl : System.Windows.Forms.Label { }            //задать контрол, который будет исп-ся для рисования (вар.2)

			int[,] TreeMap = {
				  { 1, 1, 1 },
				  { 1, 1, 1 },
				  { 1, 1, 1 },
				  { 0, 1, 0 },
				  { 0, 1, 0 } 
      };

			int[,] WaterSpotMap = {
				  { 0, 1, 1, 0 },
				  { 1, 1, 1, 1 },
				  { 0, 1, 1, 0 } 
      };

			int[,] OnePointMap = { { 1 }, { 0 } };  //ask ошибка при F11

			enum ElemType {
				 EmptyField = 0,
				 Snake = 1,
				 Fruit = 2,
				 Stone = 4,
				 Tree	 = 8,
				 Water = 16,
				 All	 = 31,
				 Any	 = -1
			}
      
      Snake snake;

			public Form1(Snake snake) {
				  InitializeComponent();
          this.snake=snake;
            //Controls["tbBrickSize"].Text = "15";
            //Controls["tbMapHeight"].Text = "30";
            //Controls["tbMapWidth"].Text = "30";
          //tbSpeed.Text = "100";
          //tbStones.Text = "7";
          //tbWater.Text = "3";
          //tbTrees.Text = "3";
          //_GenerateMap();
          ////this.KeyDown += new KeyEventHandler(OKD);
          //timer.Tick += new EventHandler(UpdateSnake);
          //StartGame();
      }
			
			#region Проверка попадания на предмет..
			//занимает ли УКАЗАННЫЙ объект точку с координатами X, Y
			private ElemType CheckArr(Point loc, UsedDrawControl[] arr, int arr_len) {
				 if (arr[0] == null) 
          return 0;
				 for (int i = 0; i < arr_len; i++) {
						if (arr[i].Location == loc)
              return ElemType.Any;
				 }
				 return 0;
			}

			//проверить, свободна ли позиция с координатами X, Y на поле
			private ElemType ChekPosition(Point loc, ElemType et) {
				 ElemType RetVal = 0;
				 //проверка попадания на змейку
				 if ((et & ElemType.Snake) > 0) {
						if (CheckArr(loc, snakeBodie,Score) !=0) 
							 RetVal |= ElemType.Snake; 
				 }
				 //проверка попадания фрукта
				 if ((et & ElemType.Fruit) > 0) {
						if (CheckArr(loc, Fruits, FruitCnt) != 0) 
							 RetVal |= ElemType.Fruit;
				 }
				 //проверка попадания на камень
				 if ((et & ElemType.Stone) > 0) {
						if (CheckArr(loc, Stones, StoneCnt) != 0) 
							 RetVal |= ElemType.Stone;
				 }
				 //проверка попадания на дерево
				 if ((et & ElemType.Tree) > 0) {
						if (CheckArr(loc, Trees, TreesCnt) != 0) 
							 RetVal |= ElemType.Tree;
				 }
				 //проверка попадания в воду
				 if ((et & ElemType.Water) > 0) {
						if (CheckArr(loc, WaterSpots, WaterCnt) != 0) 
							 RetVal |= ElemType.Water;
				 }
				 return RetVal;
			}
			#endregion

			private void UpdateSnake(object myObject, EventArgs eventArgs) {

      	  if (!form_timer.Enabled) return;
				  //новые координаты головы при движении
				  snX += dirX; snY += dirY;
				  Point loc = new Point(snX * BrickSize, snY * BrickSize);

				 //проверка выхода за границы
				 if (snX < 0 || snX == currMapWidth || snY < 0 || snY == currMapHeight) {
						form_timer.Stop();
						MessageBox.Show("Вы проиграли!");
						StartGame();
						return;
				 }

				 //потушить проглоченное яблоко, которое дошло до конца хвоста
				 if (snakeBodie[tail].Width != BrickSize) {
						snakeBodie[tail].Width = BrickSize;
						snakeBodie[tail].Height = BrickSize;
						snakeBodie[tail].BackColor = snakeColor;
						snakeBodie[tail].SendToBack();
				 }

				 //координаты уже вычислены, но элемент ещё не перемещён в новое место, поэтому сперва сделаем
				 //проверка укуса саму себя или камня
				 if (ChekPosition(loc, ElemType.Snake | ElemType.Stone) > 0) {
						form_timer.Stop();
						MessageBox.Show("Вы проиграли!");
						StartGame();
						return;
				 }

				 //переместить последний элемент змейки на место новой позиции головы
				 snakeBodie[tail].Location = loc;

				 //проверка поедания фрукта
				 if (ChekPosition(loc, ElemType.Fruit) > 0) {
						//змейка наехала на фрукт
						//head++;		//переменная не задействована
						Score++;
						lblScore.Text = Score.ToString();

						//и так, змейка наехала на фрукт. В этой точке одновременно наложены друг на друга два объекта - и фркут и голова змейки
						//т.к. размер змейки мы увеличили (score++), надо заполнить новый элемент массива. Направляем его на голову змейки (на которую сейчас указывает индекс tail)
						snakeBodie[Score] = snakeBodie[tail];

						Fruits[0].Width += BrickSize / 3;
						Fruits[0].Height += BrickSize / 3;
						Fruits[0].BackColor = snakeEatColor;
						Fruits[0].Left -= BrickSize / 6;
						Fruits[0].Top -= BrickSize / 6;
						Fruits[0].BringToFront();
						snakeBodie[tail] = Fruits[0];
						FruitCnt--;
						//разместить на карте новый фрукт
						AllocateElement(ElemType.Fruit, maxFruitCnt, Fruits, ref FruitCnt, fruitColor, OnePointMap);
						//_GenerateFruit();
				 }

				 //изменить указатель на голову и хвост, если в змейке больше 1 элемента
				 if (Score > 0) {
						//head++;
						tail++;
						//if (head == score + 1) head = 0;
						if (tail == Score + 1) tail = 0;
				 }

			}

			private void ClearObject(UsedDrawControl[] arr, ref int arr_len) {
			  //очистка массива объектов
				  if (arr[0] != null) { 
					    for (int i = arr_len; i >= 0; i--) { 
							    Controls.Remove(arr[i]);
							    arr[i] = null;
					    }
					    arr_len = 0;
				  }
			}

			private void StartGame() {
				 //очистить старую змейку и всю карту
				 ClearObject(snakeBodie, ref Score);
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
				 if (AllocateElement(ElemType.Tree,  maxTreesCnt, Trees,  ref TreesCnt, treeColor, TreeMap) != 0) { MessageBox.Show("Не удаётся разместить деревья на карте. Измените параметры"); return; }
				 if (AllocateElement(ElemType.Water, maxWaterCnt, WaterSpots, ref WaterCnt, waterColor, WaterSpotMap) != 0) { MessageBox.Show("Не удаётся разместить лужи на карте. Измените параметры"); return; }

				 //разместить змейку
				 if (AllocateElement(ElemType.Snake, 1, snakeBodie, ref Score, snakeColor, OnePointMap) != 0) { MessageBox.Show("Не удаётся разместить змейку на карте. Измените параметры"); return; }
				 Score--;    //змейку разместили и увеличили размер массива, змейка начинаться должна с 0
				 snX = snakeBodie[0].Left / BrickSize;
				 snY = snakeBodie[0].Top / BrickSize;
				 startFlag = true;

				 //snake[0] = new UsedDrawControl();
				 //snake[0].Size = new Size(BrickSize, BrickSize);
				 //snake[0].BackColor = snakeColor;
				 //snake[0].Location = new Point(snX * BrickSize, snY * BrickSize);
				 //Controls.Add(snake[0]);

				 //timer.Start();
			}
			private void btnStart_Click(object sender, EventArgs e) {
				 snake.GenerateMap();
			}

			//добавить на карту cnt элементов заданного типа. Образец карты в параметре map
			private int AllocateElement(ElemType et, int cnt, UsedDrawControl[] arr, ref int arr_len, Color color, int[,] map) {
				 Point loc;
				 Point loc_i = new Point();
				 Random r = new Random();
				 //определить параметры массива, задающую макет элемента
				 int map_width = map.GetUpperBound(1);
				 int map_height = map.GetUpperBound(0);

				 //цикл по кол-ву создаваемых элементов
				 for (int i = 0; i < cnt; i++) {
						//размещаемый элемент д.б. на пусом месте.
						int ok = 0;					 //успешное размещение всего объекта
						ElemType ok_i = 0;   //успешное размещение очередной точки объекта
						do {
							 ok += 1;   //счётчик попыток

							 //получить случайную координату лев. верхнего угла размещаемого объекта
							 loc = new Point(BrickSize * r.Next(0, currMapWidth - map_width - 1), BrickSize * r.Next(0, currMapHeight - map_height - 1));

							 //производим предварительную проверку, не попадают ли все точки элемента на что-то другое 
							 for (int path = 1; path <= 2; path++) {
									for (int y = 0; y <= map_height; y++) {
										 for (int x = 0; x <= map_width; x++) {
												//если точка не отображается, взять следующую координату
												if (map[y, x] == 0) continue;    
												loc_i.X = loc.X + x * BrickSize;
												loc_i.Y = loc.Y + y * BrickSize;
												//если проход первый, только выполняем проверку, пустое ли место
												if (path==1) {
													 //проверка попадания на другой объект
													 ok_i = ChekPosition(loc_i, ElemType.All);
													 if (ok_i > 0) break;		 //надо брать слудующую начальную координату всего объекта
												} else {
												//проход 2й - размещение элемента на карте
													 arr[arr_len] = new UsedDrawControl();
													 arr[arr_len].Size = new Size(BrickSize, BrickSize);
													 arr[arr_len].BackColor = color;
													 arr[arr_len].Location = loc_i;
													 Controls.Add(arr[arr_len]);
													 arr_len += 1;
												}
										 }
										 if (ok_i > 0) break;		 //надо брать слудующую начальную координату всего объекта
									}
									if (ok_i != 0) break;			 //надо брать слудующую начальную координату всего объекта
							 }
							 //если все точки объекта отображены, выход из цикла с попытками разместить объект
							 if (ok_i == 0) ok = 0;
						} while (ok > 0 && ok < 200);
						//не удалось разместить на карте элемент
						if (ok == 200) return -1;
				 }
				 return 0;
			}

			private void _GenerateMap() {
				 int _currMapWidth = Convert.ToInt32(this.Controls["tbMapWidth"].Text);
				 int _currMapHeight = Convert.ToInt32(Controls["tbMapHeight"].Text);
				 BrickSize = Convert.ToInt32(Controls["tbBrickSize"].Text);

				 //сперва удалить лишние горизонт и верт линии
				 for (int i = currMapHeight; i > _currMapHeight; i--) {
						Controls.Remove(LinesX[i]);
						LinesX[i] = null;
				 }
				 for (int i = currMapWidth; i > _currMapWidth; i--) {
						Controls.Remove(LinesY[i]);
						LinesY[i] = null;
				 }

				 //добавить недостающие горизонт линии
				 for (int i = currMapHeight; i < _currMapHeight; i++) {
						UsedDrawControl FieldGrid = new UsedDrawControl {
							 BackColor = Color.Black,
							 Size = new Size(1, 1),
							 Location = new Point(0, 0)
						};
						Controls.Add(FieldGrid);
						LinesX[i+1] = FieldGrid;
				 }
				 //добавить недостающие верт линии
				 for (int i = currMapWidth; i < _currMapWidth; i++) {
						UsedDrawControl FieldGrid = new UsedDrawControl {
							 BackColor = Color.Black,
							 Size = new Size(1,1),
							 Location = new Point(0,0)
						};
						Controls.Add(FieldGrid);
						LinesY[i+1] = FieldGrid;
				 }
				 currMapWidth = _currMapWidth;
				 currMapHeight = _currMapHeight;

				 //подгонка макета формы
				 Width = currMapWidth * BrickSize + ExtFieldWidth;
				 Height = (currMapHeight + 1) * BrickSize + 25;
				 
				 btnGenerateMap.Left = this.Width - ExtFieldWidth + 10;
				 btnGenerateMap.Width = ExtFieldWidth - 30;
				 lblScoreLabel.Left = this.Width - ExtFieldWidth + 10;
				 lblScore.Left = lblScoreLabel.Bounds.Right;
         /*
				 //Stones
				 lblStonesLabel.Left = this.Width - ExtFieldWidth + 5;
				 tbStones.Left = lblStonesLabel.Bounds.Right;
				 maxStoneCnt = Convert.ToInt32(tbStones.Text);
				 //Trees
				 lblTreesLabel.Left = this.Width - ExtFieldWidth + 5;
				 tbTrees.Left = lblStonesLabel.Bounds.Right;
				 maxTreesCnt = Convert.ToInt32(tbTrees.Text);
				 //Water
				 lblWaterLabel.Left = this.Width - ExtFieldWidth + 5;
				 tbWater.Left = lblStonesLabel.Bounds.Right;
				 maxWaterCnt = Convert.ToInt32(tbWater.Text);
				 //Speed
				 lblSpeedLabel.Left = this.Width - ExtFieldWidth + 3;
				 tbSpeed.Left = lblStonesLabel.Bounds.Right;
				 SnakeSpeed = Convert.ToInt32(tbSpeed.Text);
				 timer.Interval = SnakeSpeed;
         
				 //MapWidth
				 Controls["lblMapWidth"].Left = this.Width - ExtFieldWidth + 5;
				 Controls["tbMapWidth"].Left = lblStonesLabel.Bounds.Right;
				 //MapHeight
				 Controls["lblMapHeight"].Left = this.Width - ExtFieldWidth + 5;
				 Controls["tbMapHeight"].Left = lblStonesLabel.Bounds.Right;
				 //BrickSize
				 Controls["lblBrickSize"].Left = this.Width - ExtFieldWidth + 5;
				 Controls["tbBrickSize"].Left = lblStonesLabel.Bounds.Right;
         */
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

    //•ОБРАБОТЧИК НАЖАТИЯ КЛАВИШ
    private void Form1_KeyDown(object sender, KeyEventArgs e) { /*
          -задание направления движения змейки
          -если это первое нажатие
            -запуск таймера
          -----------------------------------------------------------*/
	        switch (e.KeyCode) {
              case Keys.Escape: 
                int a=1;
              break;
						  case Keys.Left: snake.dirX= -1; snake.dirY = 0; goto aa;
						  case Keys.Right: snake.dirX = 1; snake.dirY = 0; goto aa;
						  case Keys.Up: snake.dirY = -1; snake.dirX = 0;   goto aa;
						  case Keys.Down: snake.dirY = 1; snake.dirX = 0;  
           aa: 
              if (snake.startFlag) { snake.timer.Start(); startFlag = false; } 
              if (!snake.timer.Enabled) snake.timer.Start();
              break;
				  }
			}


			private void btnStart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
				 //контролы перехватывают на себя спец клавиши и они в форму не доходят, поэтому...
				 switch (e.KeyCode) {
						case Keys.Down:
						case Keys.Up:
						case Keys.Left:
						case Keys.Right:
							 e.IsInputKey = true;
							 break;
				 }
			}

			//private void OKD(object sender, KeyEventArgs e) { 
			//    switch(e.KeyCode.ToString()) {   
			//        case "Right":   dirX = 1;   dirY = 0;   break;
			//        case "Left":    dirX = -1;  dirY = 0;   break;
			//        case "Up":      dirY = -1;  dirX = 0;   break;
			//        case "Down":    dirY = 1;   dirX = 0;   break;
			//    }
			//}

			//private void _GenerateFruit() {
			//	 //определить добавляемое кол-во фруктов 
			//	 //определить, макс.кол-во сколько вообще возможно добавить
			//	 int RestFrCnt = maxFruitCnt - FruitCnt;

			//	 //взять случ.число - добавляемое кол-во фруктов 
			//	 Random r = new Random();
			//	 int addFruitCnt = r.Next(0, RestFrCnt + 1);
			//	 //чтобы был добавлен хотя бы 1 фрукт, если их нет
			//	 if (addFruitCnt == 0 && FruitCnt == 0) addFruitCnt = 1;

			//	 //добавить количество фруктов в размере addFruitCnt
			//	 for (int i = FruitCnt; i < FruitCnt + addFruitCnt; i++) {
			//			fruit[i] = new UsedDrawControl();
			//			fruit[i].Size = new Size(_BrickSize, _BrickSize);
			//			fruit[i].BackColor = fruitColor;

			//			//фрукты не должны попадать на змейку, камни, воду, деревья или другой фрукт
			//			bool ok = false; Point loc;
			//			do {
			//				 loc = new Point(_BrickSize * r.Next(0, _currMapWidth), _BrickSize * r.Next(0, _currMapHeight));
			//				 //проверка попадания фрукта на змейку, камень, дерево, воду или другой фрукт
			//				 if (ChekPosition(loc, ElemType.All) == 0) ok = true;
			//			} while (!ok);
			//			fruit[i].Location = loc;
			//			Controls.Add(fruit[i]);
			//	 }
			//	 FruitCnt += addFruitCnt;
			//}

	 }
}
