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

namespace nsSnake {
	 public partial class Form1 : Form {

			public int maxFruitCnt = 1;
			public int maxStoneCnt = 3;
			public int maxTreesCnt = 2;
			public int maxWaterCnt = 2;

			public Color snakeColor = Color.Gray;
			public Color snakeEatColor = Color.DarkRed;
			public Color fruitColor = Color.Red;
			public Color treeColor = Color.Green;
			public Color stoneColor = Color.Black;
			public Color waterColor = Color.Blue;

			public int _BrickSize = 10;
			public int _MapSizeX = 40;
			public int _MapSizeY = 40;
			public int _ExtFieldWidth = 100;
			public int _snakeSpeed = 100;

			private int score;         //длина змейки
			private int FruitCnt ;
			private int StoneCnt ;
			private int TreesCnt ;
			private int WaterCnt ;

			private UsedDrawControl[] fruit = new UsedDrawControl[10];
			private UsedDrawControl[] snake = new UsedDrawControl[100];
			private UsedDrawControl[] WaterSpot = new UsedDrawControl[60];
			private UsedDrawControl[] Trees = new UsedDrawControl[60];
			private UsedDrawControl[] Stones = new UsedDrawControl[10];
			private UsedDrawControl[] LinesX = new UsedDrawControl[200];
			private UsedDrawControl[] LinesY = new UsedDrawControl[200];

			private int dirX, dirY;         //коеф. умножения (-1, +1), задающие направление движения 
			private int snX, snY;           //позиция головы змейки
			private int tail = 0;           //позиция хвоста
			//private int head = 0;           //позиция головы
			//private class UsedDrawControl : System.Windows.Forms.PictureBox { }       //задать контрол, который будет исп-ся для рисования (вар.2)
			//private class UsedDrawControl : System.Windows.Forms.Label { }            //задать контрол, который будет исп-ся для рисования (вар.2)

			int[,] TreeMap =
				{{ 1, 1, 1 },
				 { 1, 1, 1 },
				 { 1, 1, 1 },
				 { 0, 1, 0 },
				 { 0, 1, 0 }};

			int[,] WaterSpotMap =
				{{ 0, 1, 1, 0 },
				 { 1, 1, 1, 1 },
				 { 0, 1, 1, 0 }};

			int[,] OnePointMap = { { 1 }, { 0 } };

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

			public Form1() {
				 InitializeComponent();
				 tbBrickSize.Text = _BrickSize.ToString();
				 tbMapHeight.Text = _MapSizeY.ToString();
				 tbMapWidth.Text = _MapSizeX.ToString();
				 tbSpeed.Text = _snakeSpeed.ToString();
				 tbWater.Text = maxWaterCnt.ToString();
				 tbStones.Text = maxStoneCnt.ToString();
				 tbTrees.Text = maxTreesCnt.ToString();
				 _GenerateMap();
				 //this.KeyDown += new KeyEventHandler(OKD);
				 timer.Tick += new EventHandler(UpdateSnake);
			}

			//занимает ли УКАЗАННЫЙ объект точку с координатами X, Y 
			private ElemType CheckArr(Point loc, UsedDrawControl[] arr, int arr_len) {
				 if (arr[0] == null) return 0;
				 for (int i = 0; i < arr_len; i++) {
						if (arr[i].Location == loc) return ElemType.Any;
				 }
				 return 0;
			}

			//метод проверяет, свободна ли позиция с координатами X, Y на поле
			private ElemType ChekPosition(Point loc, ElemType et) {
				 ElemType RetVal = 0;
				 //проверка попадания на змейку
				 if ((et & ElemType.Snake) > 0) {
						if (CheckArr(loc, snake,score) !=0) 
							 RetVal |= ElemType.Snake; 
				 }
				 //проверка поедания фрукта
				 if ((et & ElemType.Fruit) > 0) {
						if (CheckArr(loc, fruit, FruitCnt) != 0) 
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
						if (CheckArr(loc, WaterSpot, WaterCnt) != 0) 
							 RetVal |= ElemType.Water;
				 }
				 return RetVal;
			}

			private void UpdateSnake(object myObject, EventArgs eventArgs) {
				 //новые координаты головы при движении
				 snX += dirX; snY += dirY;
				 Point loc = new Point(snX * _BrickSize, snY * _BrickSize);

				 //проверка выхода за границы
				 if (snX < 0 || snX == _MapSizeX || snY < 0 || snY == _MapSizeY) {
						timer.Stop();
						MessageBox.Show("Вы проиграли!");
				 }

				 //потушить проглоченное яблоко, которое дошло до конца хвоста
				 if (snake[tail].Width != _BrickSize) {
						snake[tail].Width = _BrickSize;
						snake[tail].Height = _BrickSize;
						snake[tail].BackColor = snakeColor;
						snake[tail].SendToBack();
				 }

				 //координаты уже вычислены, но элемент ещё не перемещён в новое место, поэтому сперва сделаем
				 //проверка укуса саму себя или камня
				 if (ChekPosition(loc, ElemType.Snake | ElemType.Stone) > 0) {
						timer.Stop();
						MessageBox.Show("Вы проиграли!");
				 }

				 //переместить последний элемент змейки на место новой позиции головы
				 snake[tail].Location = loc;

				 //проверка поедания фрукта
				 if (ChekPosition(loc, ElemType.Fruit) > 0) {
						//змейка наехала на фрукт
						//head++;		//переменная не задействована
						score++;
						lblScore.Text = score.ToString();

						snake[score] = fruit[0];
						snake[score].Width += _BrickSize / 4;
						snake[score].Height += _BrickSize / 4;
						snake[score].BackColor = snakeEatColor;
						snake[score].Left -= _BrickSize / 8;
						snake[score].Top -= _BrickSize / 8;
						snake[score].BringToFront();
						FruitCnt--;
						//разместить на карте новый фрукт
						int a = AllocateElement(ElemType.Fruit, maxFruitCnt, fruit, ref FruitCnt, fruitColor, OnePointMap);
						//_GenerateFruit();
				 }

				 //изменить указатель на голову и хвост, если в змейке больше 1 элемента
				 if (score > 0) {
						//head++;
						tail++;
						//if (head == score + 1) head = 0;
						if (tail == score + 1) tail = 0;
				 }

			}

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
			//				 loc = new Point(_BrickSize * r.Next(0, _MapSizeX), _BrickSize * r.Next(0, _MapSizeY));
			//				 //проверка попадания фрукта на змейку, камень, дерево, воду или другой фрукт
			//				 if (ChekPosition(loc, ElemType.All) == 0) ok = true;
			//			} while (!ok);
			//			fruit[i].Location = loc;
			//			Controls.Add(fruit[i]);
			//	 }
			//	 FruitCnt += addFruitCnt;
			//}

			private void ClearObject(UsedDrawControl[] arr, ref int arr_len) {
				 //очистка массива объектов
				 if (arr[0] != null) { 
						for (int i = arr_len; i >= 0; i--) { 
							 Controls.Remove(arr[i]);
						}
						arr_len = 0;
				 }
			}

			private void btnStart_Click(object sender, EventArgs e) {
				 _GenerateMap();
				 //очистить старую змейку и всю карту
				 ClearObject(snake, ref score);
				 ClearObject(fruit, ref FruitCnt);
				 ClearObject(Trees, ref TreesCnt);
				 ClearObject(Stones, ref StoneCnt);
				 ClearObject(WaterSpot, ref WaterCnt);

				 //подготовиться к старту сначала
				 dirX = 1; dirY = 0; tail = 0; score = 0; //head = 0;
				 snX = _MapSizeX / 2;
				 snY = _MapSizeY / 2;
				 lblScore.Text = "0";

				 //разместить все объекты на карте
				 if (AllocateElement(ElemType.Fruit, maxFruitCnt, fruit, ref FruitCnt, fruitColor, OnePointMap) != 0) { MessageBox.Show("Не удаётся разместить фрукты на карте. Измените параметры"); return; }
				 if (AllocateElement(ElemType.Stone, maxStoneCnt, Stones, ref StoneCnt, stoneColor,  OnePointMap) != 0) { MessageBox.Show("Не удаётся разместить камни на карте. Измените параметры"); return; }
				 if (AllocateElement(ElemType.Tree, maxTreesCnt, Trees, ref TreesCnt, treeColor, TreeMap) != 0) { MessageBox.Show("Не удаётся разместить деревья на карте. Измените параметры"); return; }
				 if (AllocateElement(ElemType.Water, maxWaterCnt, WaterSpot, ref WaterCnt, waterColor, WaterSpotMap) != 0) { MessageBox.Show("Не удаётся разместить лужи на карте. Измените параметры"); return; }

				 //разместить змейку
				 snake[0] = new UsedDrawControl();
				 snake[0].Size = new Size(_BrickSize, _BrickSize);
				 snake[0].BackColor = snakeColor;
				 snake[0].Location = new Point(snX * _BrickSize, snY * _BrickSize);
				 Controls.Add(snake[0]);

				 timer.Interval = _snakeSpeed;
				 timer.Start();
			}

			//добавить на карту cnt элементов заданного типа. Образец карты в параметре map
			private int AllocateElement(ElemType et, int cnt, UsedDrawControl[] arr, ref int arr_len, Color color, int[,] map) {
				 Point loc;
				 Point loc_i = new Point();
				 Random r = new Random();
				 //определить параметры массива, задающую карты
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
							 loc = new Point(_BrickSize * r.Next(0, _MapSizeX - map_width - 1), _BrickSize * r.Next(0, _MapSizeY - map_height - 1));

							 //производим предварительную проверку, не попадают ли все точки элемента на что-то другое 
							 for (int path = 1; path <= 2; path++) {
									for (int y = 0; y <= map_height; y++) {
										 for (int x = 0; x <= map_width; x++) {
												//если точка не отображается, взять следующую координату
												if (map[y, x] == 0) continue;    
												loc_i.X = loc.X + x * _BrickSize;
												loc_i.Y = loc.Y + y * _BrickSize;
												//если проход первый, только выполняем проверку, пустое ли место
												if (path==1) {
													 //проверка попадания на другой объект
													 ok_i = ChekPosition(loc_i, ElemType.All);
													 if (ok_i > 0) break;		 //надо брать слудующую начальную координату всего объекта
												} else {
													 arr[arr_len] = new UsedDrawControl();
													 arr[arr_len].Size = new Size(_BrickSize, _BrickSize);
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
				 //сперва удалить имеющиеся горизонт и верт линии
				 for (int i = _MapSizeX; i > 0; i--) {
						this.Controls.Remove(LinesX[i]);
				 }
				 for (int i = _MapSizeY; i > 0; i--) {
						this.Controls.Remove(LinesY[i]);
				 }
				 _BrickSize = Convert.ToInt32(tbBrickSize.Text);
				 _MapSizeX = Convert.ToInt32(tbMapWidth.Text);
				 _MapSizeY = Convert.ToInt32(tbMapHeight.Text);

				 //подгонка макета формы
				 Width = _MapSizeX * _BrickSize + _ExtFieldWidth;
				 Height = (_MapSizeY + 1) * _BrickSize + 25;
				 btnStart.Left = this.Width - _ExtFieldWidth + 10;
				 btnStart.Width = _ExtFieldWidth - 30;
				 lblScoreLabel.Left = this.Width - _ExtFieldWidth + 10;
				 lblScore.Left = lblScoreLabel.Bounds.Right;

				 //Stones
				 lblStonesLabel.Left = this.Width - _ExtFieldWidth + 5;
				 tbStones.Left = lblStonesLabel.Bounds.Right;
				 maxStoneCnt = Convert.ToInt32(tbStones.Text);
				 //Trees
				 lblTreesLabel.Left = this.Width - _ExtFieldWidth + 5;
				 tbTrees.Left = lblStonesLabel.Bounds.Right;
				 maxTreesCnt = Convert.ToInt32(tbTrees.Text);
				 //Water
				 lblWaterLabel.Left = this.Width - _ExtFieldWidth + 5;
				 tbWater.Left = lblStonesLabel.Bounds.Right;
				 maxWaterCnt = Convert.ToInt32(tbWater.Text);
				 //Speed
				 lblSpeedLabel.Left = this.Width - _ExtFieldWidth + 3;
				 tbSpeed.Left = lblStonesLabel.Bounds.Right;
				 _snakeSpeed = Convert.ToInt32(tbSpeed.Text);

				 //MapWidth
				 lblMapX.Left = this.Width - _ExtFieldWidth + 5;
				 tbMapWidth.Left = lblStonesLabel.Bounds.Right;
				 //MapHeight
				 lblMapY.Left = this.Width - _ExtFieldWidth + 5;
				 tbMapHeight.Left = lblStonesLabel.Bounds.Right;
				 //BrickSize
				 lblBrickSize.Left = this.Width - _ExtFieldWidth + 5;
				 tbBrickSize.Left = lblStonesLabel.Bounds.Right;

				 //Рисуем горизонт линии
				 for (int i = 0; i <= _MapSizeY; i++) {
						UsedDrawControl FieldGrid = new UsedDrawControl {
						  BackColor = Color.Black,
						  Size = new Size(_MapSizeX * _BrickSize, 1),
						  Location = new Point(0, i * _BrickSize)
						};
						this.Controls.Add(FieldGrid);
						LinesX[i] = FieldGrid;
				 }

				 //Рисуем вертик линии
				 _MapSizeY = Convert.ToInt32(tbMapHeight.Text);
				 for (int i = 0; i <= _MapSizeX; i++) {
						UsedDrawControl FieldGrid = new UsedDrawControl {
							 BackColor = Color.Black,
							 Size = new Size(1, _MapSizeY * _BrickSize),
							 Location = new Point(i * _BrickSize)
						};
						this.Controls.Add(FieldGrid);
						LinesY[i] = FieldGrid;
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

			private void Form1_KeyDown(object sender, KeyEventArgs e) {
				 switch (e.KeyCode) {
						case Keys.Left: dirX = -1; dirY = 0; break;
						case Keys.Right: dirX = 1; dirY = 0; break;
						case Keys.Up: dirY = -1; dirX = 0; break;
						case Keys.Down: dirY = 1; dirX = 0; break;
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

	 }
}
