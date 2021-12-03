using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using UsedDrawControl = System.Windows.Forms.PictureBox;  //задать контрол, который будет исп-ся для рисования
using UsedDrawControl = System.Windows.Forms.Label;         //задать контрол, который будет исп-ся для рисования

namespace Snake {
    public enum GI { Form };
    //

    public class Snake {

        #region ОТКРЫТЫЕ ПАРАМЕТРЫ ИГРЫ  
        //ПАРАМЕТРЫ ИГРЫ ......................................................
          public GI gi = GI.Form;                     //режим граф. интерфейса
          private int maxFruitCnt = 1;
          public int MaxFruitCnt { get=> maxFruitCnt; set=> maxFruitCnt=value; }    //кол-во фруктов на поле (пока что больше не поддерживается)
          public int SnakeSpeed  { get; set; } = 150; //скорость змейки

        /*ПАРАМЕТРЫ ИГРОВОГО ПОЛЯ..............................................*/
          //размеры поля
            private int mapWidth  = 30; 
            private int mapHeight = 30;
            private static int brickSize = 18;       //размер ячейки
            //
            public  int MapWidth   { get => mapWidth;   set => mapWidth = value; } 
            public  int MapHeight  { get => mapHeight;  set => mapHeight = value; } 
            public  int BrickSize  { get => brickSize;  set { 
                  if (value < 5)
                    brickSize = 5;
                  else if (value > MapWidth/10)
                    brickSize = MapWidth/10;
                  else
                    brickSize = value;
                }
            }

        /*ПАРАМЕТРЫ ВНЕШНЕГО ВИДА..............................................*/
            public readonly int ExtFieldWidth = 110;     //размер служебного поля на форме
          //цвета
            public Color SnakeColor   { get; set; } = Color.Gray;
            public Color SnakeEatColor{ get; set; } = Color.Gray;
            public Color FruitColor   { get; set; } = Color.Red;
        /*--------------------------------------------------------------------*/
        #endregion

        #region СЛУЖЕБНЫЕ ПАРАМЕТРЫ ОБЕСПЕЧЕНИЯ РАБОТОСПОСОБНОСТИ
          private static Form form;   //ask почему form.Controls.Remove(item) form.Controls.Add() требовал static
          public  Form GetForm { get => form; } //возвращает ссылку на форму
          //
          private int currMapWidth = -1;        //кол-во линий должно быть на 1 больше, чем кол-во клеток 
          private int currMapHeight = -1;
    
          private int score;                //длина змейки
          private int fruitCnt;             //осталось фруктов на поле
          private int headX, headY;         //позиция головы змейки
          private int tail = 0;             //позиция хвоста
          internal int dirX, dirY;          //коеф. умножения (-1, +1), задающие направление движения
          internal bool startFlag = false;  //флаг начала игры
          private ElementsAtXY elementsAtXY = new();//

          #if TEST
          //метки на карте на хвост, голову и название элементов элементы
            private UsedDrawControl lbl_t;
            private static Size testLabelSize = new (8, 8);   //ask why static?
            private static Dictionary<int,Label> overlapedIDDict = new();
            private bool testMode = true;
          #else
            private bool testMode = false;
          #endif

          private static ElementBitPosition[,] allocatedElements;
          private UsedDrawControl[] snakeArr= new UsedDrawControl[100];         //массив элементов змейки
          private UsedDrawControl[] fruits  = new UsedDrawControl[10];          //массив фруктов
          private UsedDrawControl[] linesX  = new UsedDrawControl[200];         //массивы линий
          private UsedDrawControl[] linesY  = new UsedDrawControl[200];         //mbsim may be simplified

        //размеры элементов управления
          private int newControl_Ypos;            //позиция Y для добавления очередного элемента управления на форму
          private int newControl_Xpos;            //позиция Y для добавления очередного элемента управления на форму
          private int newCntrol_LabelWidth = 70;  //ширина подписи
          private int newCntrol_LabelHeight= 20;  //ширина подписи
          private int newCntrol_TextBoxWidth;     //ширина поля ввода (расчит-ся автомат)
          
          public enum ElementBitPosition {
				     EmptyField = 0,
				     Snake = 1,
             EatenFruit = 2,
				     Fruit = 4,
				     Stone = 8,
				     Tree	 = 16,
				     Water = 32,
			    }
        
          private int[,] onePointMap = { { 1, 0 }, { 0, 0 } };
        
          public enum ElementID {
				     Snake = 0,
             EatenFruit=1,
				     Fruit = 2,
				     Stone = 3,
				     Tree	 = 4,
				     Water = 5,
          }
          private static int[,] allowOverlap = {
            /*------ Snake EatnFr Fruit Stone Tree Water */
            /*Snake*/ {0,   0,     0,    0,    1,   1},
            /*EatnFr*/{0,   0,     0,    0,    1,   1},
            /*Fruit*/ {0,   0,     0,    0,    1,   1},
            /*Stone*/ {0,   0,     0,    0,    1,   1},
            /*Tree */ {1,   1,     1,    1,    1,   1},
            /*Water*/ {1,   1,     1,    1,    1,   1},
          };
        
          internal Timer timer = new Timer();

        #if TEST
          private int[,] FruitPoints =  { {4,0}, {6,0}, {9,0}, {13,0}, {16,0} };
          private Tuple<int, int>[] FruitPoints2 = new Tuple<int, int>[] {
              Tuple.Create(14, 0), 
              Tuple.Create(16, 0), 
              Tuple.Create(18, 0), 
              Tuple.Create(20, 0), 
              Tuple.Create(22, 0)
          };  
          private int fruitPt_i = 0;
        #endif
        #endregion
        
        /*------------------------------------------------------------------------------------------------------------------*/
        #region ВСПОМОГАТЕЛЬНЫЕ ПРОЦЕДУРЫ (не влияющие на логику работы) 
          /*------------------------------------------------------------------------------------------------------------------*/
          private struct ElementsAtXY {
              //список элементов в заданном знакоместе
                public List<ElementID> list;
              //флаги уст элем-тов в заданном знакоместе
                private int bits;
                public  int Bits { get=>bits; }
              //координаты элемента в массиве карты
                private Point xy;

                public bool this [ElementBitPosition index, ElementBitPosition index2 = ElementBitPosition.EmptyField] {
                    get {
                        bool res=(bits & (int)index) != 0;
                        if (index2 != ElementBitPosition.EmptyField) 
                          res |= (bits & (int)index2) != 0;
                        return res;
                    }
                    set {
                        if (value) 
                        //установка бита
                            bits |= (int)index;
                        else
                        //сброс бита
                            bits &= ~((int)index);

                      //сохранить изменённое значение
                        allocatedElements[xy.Y, xy.X] = (ElementBitPosition)bits;
                    }
                }
              /*------------------------------------------------------------------------------------------------------------------*/
              /*Получить список объектов в позиции X,Y */                 
                public void Read(Point loc, bool showElements = false) {    //private IEnumerable<Elements> ElementsAtXY(Point loc) { 
                  //вместо конструктора
                    list ??= new List<ElementID>();
                    xy = loc;

                  //подготовка к создание нового списка элементов
                    list.Clear();

                  //получить элемент(ы), занимающие позицию X,Y
                    bits = (int)allocatedElements[loc.Y,loc.X];
                    int bits_tmp = bits;

                  //пройтись по этим элементам в данной точке..
                    int j=1;    //очередной элемент в точке XY
                    int k=1;    //располагать его выше/ниже
                    int i=0;
                    while (bits_tmp>0) {
                      //i - перебирает элементы от 0 и дальше..
                      //делим value на 2, смотрим остаток
                      //если остаток есть, значит i-элемент размещён в данн позиции
                        int a = bits_tmp % 2;
                        if (a>0) {          //yield return (Elements)i;
                            //если остаток есть, значит бит установлен
                            list.Add((ElementID)i);
                        }
                        bits_tmp /= 2;
                        i+=1;
                    }
  			        }
              /*------------------------------------------------------------------------------------------------------------------*/
              /*Проверить,разрешено ли добавление элемента в позицию XY */
                public bool IsOverlapAllowed(ElementID element) {
                  //пройтись по этим элементам в данной точке..
                    if (list == null) return true;
                    foreach (var item in list) {
                        //проверить по справоч таблице, разрешается ли элементу перекрытие с заданым
                        if (allowOverlap[(int)item, (int)element] == 0) return false;
                    }
				            return true;
			          }
              /*------------------------------------------------------------------------------------------------------------------*/
          }
          /*------------------------------------------------------------------------------------------------------------------*/

          /*------------------------------------------------------------------------------------------------------------------*/
          /*Добавление эл.управл. на форму */
            private int AddControl(string name, int value) {
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
                form.Controls["grParams"].Controls.Add(lb);
              //добавление поля ввода
                TextBox tb = new TextBox { 
                    Name = "tb" + name.Replace(" ", ""),
                    Width = newCntrol_TextBoxWidth,
                    Height= newCntrol_LabelHeight,
                    Text = value.ToString(),
                    AutoSize = false,
                    TextAlign =  HorizontalAlignment.Left,
                    Location = new Point(newControl_Xpos + newCntrol_LabelWidth - 5, newControl_Ypos)
                };
                form.Controls["grParams"].Controls.Add(tb);
                tb.BringToFront();

                newControl_Ypos += tb.Height + 1;
                return 0;
            }
          /*------------------------------------------------------------------------------------------------------------------*/
          /*Чтение значения с формы */
            private void TakeValue(string controlName, ref int parameterName) {
                if (int.TryParse(form.Controls["tb"+controlName].Text, out int tmpValue)) {
                    parameterName = tmpValue;
                    if (form.Controls["tb"+controlName].BackColor == Color.Tomato) 
                        form.Controls["tb"+controlName].BackColor=Control.DefaultBackColor; }
                else {
                    form.Controls["tb"+controlName].BackColor=Color.Tomato; 
                }
            }
          /*------------------------------------------------------------------------------------------------------------------*/
        #endregion 

        /*------------------------------------------------------------------------------------------------------------------*/
        /*КОНСТРУКТОР КЛАССА *//* 
          -создать форму и получение ссылки на форму
          -подготовить форму к размещению элементов:
            -добавление основных элементов управления на форму
            -вызов делегата размещения других элементов управления
            -Генерация карты
          -назначание обработчика события нажатия клавиши
          -----------------------------------------------------------*/
          public Snake() {
              //если режим гафики - элементы управления
              if (gi == GI.Form) {
                //создать форму и запомнить ссылку на неё
                  form = new Form1(this);
                  //обработчки клавиш управления в коде формы

                //подготовиться к размещению доп. элементов
                  var btn = form.Controls["grParams"].Controls["btnGenerateMap"];
                  newControl_Xpos = btn.Location.X;
                  newControl_Ypos = btn.Location.Y+btn.Height + 15;
                  //newCntrol_LabelWidth  = btn.Width;
                  newCntrol_TextBoxWidth= ExtFieldWidth-newCntrol_LabelWidth-5;
                
                //добавление основных элементов управления на форму
                  AddControl ("MapWidth", MapWidth);
                  AddControl ("MapHeight", MapHeight);
                  AddControl ("Brick size", BrickSize);
                  AddControl ("Fruit cnt", MaxFruitCnt);
                  AddControl ("Speed", SnakeSpeed);
              }
              if (!GenerateMap()) {
                /*неуспешное размещение элементов*/ }
              else {
                //разрешение запустить игру(таймер) по нажатию клавиш курсура
                  startFlag = true;
                  timer.Tick += Timer_Tick;
              }
          }
        /*------------------------------------------------------------------------------------------------------------------*/

        /*------------------------------------------------------------------------------------------------------------------*/
        /*ОБРАБОТЧИК ВЫЗОВА ОТ ТАЙМЕРА */ /*
          •хвост должен изменить координаты
          -проверка выхода за ганицы новых координат хвоста (головы)
          -перемещение хвоста змейки
          -анализ ячейки нового расположения головы
            >поедание фрукта
            >вызов других процедур
          -----------------------------------------------------*/
          private void Timer_Tick(object sender, EventArgs e) {
              Label tmpLabel=null;

            //координаты хвоста
				      int tailX = snakeArr[tail].Location.X / brickSize; 
              int tailY = snakeArr[tail].Location.Y / brickSize;
        	  //новые координаты головы при движении
              headX += dirX;  headY += dirY;
				      Point loc_MapArr = new Point(headX, headY);
				      Point loc_Screen = new Point(headX * BrickSize, headY * BrickSize);

				    //•ПРОВЕРКА ВЫХОДА ЗА ГРАНИЦЫ
				      if (headX < 0 || headX == currMapWidth || headY < 0 || headY == currMapHeight) {
						      timer.Stop();
						      MessageBox.Show("Вы проиграли!");
						      //StartGame();
						      return;
				      }

				    //•ПОТУШИТЬ ПРОГЛОЧЕННОЕ ЯБЛОКО, КОТОРОЕ ДОШЛО ДО КОНЦА ХВОСТА
				      if (snakeArr[tail].Width != BrickSize) {
                //восстановить размеры
						      snakeArr[tail].Width = BrickSize;
						      snakeArr[tail].Height = BrickSize;
						      snakeArr[tail].BackColor = SnakeColor;
						      snakeArr[tail].SendToBack();
                //очистить бит "проглоченное яблоко"
                  allocatedElements[tailY, tailX] &= ~ElementBitPosition.EatenFruit;
				      }

              //список элементов в новой позиции головы
                elementsAtXY.Read(loc_MapArr, testMode);

            //•ПРВЕРКА ПЕРЕСЕЧЕНИЯ С ДРУГИМ ПРЕДМЕТОМ
              //if (elementsAtXY.list.Count > 0) {
              if (elementsAtXY.Bits > 0) {

				        //ПРОВЕРКА УКУСА САМУ СЕБЯ
                  //if (elementsAtXY.list.Contains(Elements.Snake)) {
                  if (elementsAtXY[ElementBitPosition.Snake, ElementBitPosition.EatenFruit]) {
						          timer.Stop();
						          MessageBox.Show("Вы проиграли!");
						          //StartGame();
						          return;
				          }

				        //ПРОВЕРКА ПОЕДАНИЯ ФРУКТА
                  if (elementsAtXY[ElementBitPosition.Fruit]) {
				            //if (elementsAtXY.list.Contains(Elements.Fruit)) {

                    /*ЛОГИКА РАБОТЫ*/ {/*
                    * крутится счётчик tail, указывающий какой из элементов массива в данный момент является самым старым (хвостом) и требует перемещения. 
                    * До этого момента голова находилась в элементе snake[tail-1], сейчас ей надо переместить в [tail]
                    * и так, змейка наезжает на фрукт. В этот момент:
                    * - увеличился размер массива змейки (score++);
                    * - на этом новом месте нужно разместить бывший фрукт, который превращается в элемент змейки;
                    * - координаты хвоста приравнять к координатам фрукта, можно увеличить его в размере;
                    * Когда tail станет равен score - это будет момент визуального увеличения змейки () и один из двух элементов, находящиеся на одном месте, переместится в изголовье.
						        */}

                    //ПРЕВРАТИТЬ ФРУКТ В ЗМЕЙКУ
                      //хвост превращается в проглоченный фрукт на месте головы (перекрывая проглоченный фрукт)
                        //SnakeArr[tail].Location = loc_Screen;   //переместится позже
						            snakeArr[tail].BackColor = SnakeEatColor;
						            snakeArr[tail].Width += BrickSize / 3;
						            snakeArr[tail].Height += BrickSize / 3;
						            snakeArr[tail].Left -= BrickSize / 6;
						            snakeArr[tail].Top -= BrickSize / 6;
						            snakeArr[tail].BringToFront();

                      //увеличение массива элементов змейки
                        score++;
                        form.Controls["grParams"].Controls["lblScore"].Text = score.ToString();
                      //в новом месте массива змейки (score) разместить бывший фрукт
                        snakeArr[score] = fruits[0];
						            fruitCnt--;
                      //изменить форматирование и флаг
						            fruits[0].BackColor = SnakeColor;
                        elementsAtXY[ElementBitPosition.Fruit] = false;
                        elementsAtXY[ElementBitPosition.EatenFruit] = true;
                        //allocatedElements[headY, headX] ^= (MapElement)Math.Pow(2,(int)Elements.Fruit);   //ask
                        //allocatedElements[headY, headX] |= (MapElement)Math.Pow(2,(int)Elements.Snake);

                      #if TEST 
                        //перечитать содержимое элементов в месте головы
                          elementsAtXY.Read(loc_MapArr, testMode);

                        //отобразить номер элемента массива snakeArr
                          fruits[0].Text = score.ToString();
                          fruits[0].Font = new Font ("Arial", 6);
                          fruits[0].BringToFront();

                        //cоздать метку с индексом элемента, который перекрыт
                          tmpLabel = new Label {
                              Location = new Point (snakeArr[score].Location.X + snakeArr[tail].Size.Width-testLabelSize.Width, snakeArr[score].Location.Y + snakeArr[tail].Size.Height - testLabelSize.Height),
                              Size = testLabelSize,
                              Text = score.ToString(),
                              BackColor = SnakeColor,
                              Font = new Font("Arial", 6)
                          };
                          overlapedIDDict.Add(score, tmpLabel);
                          form.Controls.Add(tmpLabel);
                      #endif

						        //РАЗМЕСТИТЬ НА КАРТЕ НОВЫЙ ФРУКТ 
                      #if TEST
                        //ask CustomPoint = new Point((int,int)FruitPoints2[fruitPt_i]);    //как восп-ся кортежем?
                        CustomPoint = new (FruitPoints[fruitPt_i,0], FruitPoints[fruitPt_i++,1]); 
                        if (fruitPt_i==5) fruitPt_i=0;
                      #endif
						          AllocateElement(ElementID.Fruit, maxFruitCnt, fruits, ref fruitCnt, FruitColor, onePointMap); }
                  else {
                  }
              }

				    //•ПЕРЕМЕСТИТЬ ХВОСТ И НАЗНАЧИТЬ ХВОСТОМ СЛЕДУЮЩИЙ ЭЛЕМЕНТ
				      //переместить хвост на место новой позиции головы
				          snakeArr[tail].Location = loc_Screen;
                  snakeArr[tail].BringToFront();
                //отображение изменений в массиве карты
                  allocatedElements[tailY, tailX] &= ~ElementBitPosition.Snake;
                  allocatedElements[headY, headX] |=  ElementBitPosition.Snake;

                #if TEST
                  if (overlapedIDDict.ContainsKey(tail)) {
                    form.Controls.Remove(overlapedIDDict[tail]);
                    overlapedIDDict.Remove(tail);
                  }
                #endif

				        //крутим счётчик счётчик, указывающий на хвост змейки
						      if (tail == score) tail = 0;
                  else tail++;

              //обозначить голову и хвост
                #if TEST
                  lbl_t.Location = new Point (snakeArr[tail].Location.X, snakeArr[tail].Location.Y+snakeArr[tail].Size.Height-testLabelSize.Height);
                  lbl_t.Text = tail.ToString();
                  lbl_t.BringToFront();
                  //elementsAtXY.Read(loc_MapArr, testMode);
                  
                  if (tmpLabel != null) tmpLabel.BringToFront();
                #endif //
          }
        /*------------------------------------------------------------------------------------------------------------------*/

        /*------------------------------------------------------------------------------------------------------------------*/
        /*ГЕНЕРАЦИЯ КАРТЫ *//*
          -вычисление и задание размера формы
          -прорисовка сетки поля
          -расположение элементов игры
          -вызов делегата расположения элементов игры  
        ----------------------------------------------*/
          private bool GenerateMap() { 
            //режим Формы 
              //ВЫЧИСЛЕНИЕ И ЗАДАНИЕ РАЗМЕРА ФОРМЫ
                form.Width  = (MapWidth)* BrickSize + ExtFieldWidth + 15;
                form.Height = (MapHeight + 1)* BrickSize + 25;

              /*ПРОРИСОВКА СЕТКИ*/ {
                //сперва удалить лишние горизонт и верт линии
                //  если есть лишние линии, то currMapHeight будет больше MapHeight
                //  цикл в сторону уменьшения, удаляем последние линии
                  for (int i = currMapHeight; i > MapHeight; i--) {
                    form.Controls.Remove(linesX[i]);
                    linesX[i] = null;
                  }
                  for (int i = currMapWidth; i > MapWidth; i--) {
                    form.Controls.Remove(linesY[i]);
                    linesY[i] = null;
                  }

                //добавить недостающие горизонт линии
                  for (int i = MapHeight; i > currMapHeight; i--) {
                      UsedDrawControl fieldGrid = new UsedDrawControl {
                        BackColor = Color.Black,
                        Size = new Size(1, 1),
                        Location = new Point(0, 0)
                      };
                      form.Controls.Add(fieldGrid);
                      linesX[i] = fieldGrid;
                  }
                //добавить недостающие верт линии
                  for (int i = MapWidth; i > currMapWidth; i--) {
                      UsedDrawControl FieldGrid = new UsedDrawControl {
                        BackColor = Color.Black,
                        Size = new Size(1,1),
                        Location = new Point(0,0)
                      };
                      form.Controls.Add(FieldGrid);
                      linesY[i] = FieldGrid;
                  }
                  currMapWidth = MapWidth;
                  currMapHeight = MapHeight;

                //правим размеры горизонт линии
                  for (int i = 0; i <= currMapHeight; i++) {
                    linesX[i].Size = new Size(currMapWidth * BrickSize, 1);
                    linesX[i].Location = new Point(0, i * BrickSize);
                  }
                //правим размеры вертик линии
                  for (int i = 0; i <= currMapWidth; i++) {
                    linesY[i].Size = new Size(1, currMapHeight * BrickSize);
                    linesY[i].Location = new Point(i * BrickSize, 0);
                  } 
              }
              
              /*РАСПОЛОЖЕНИЕ ЭЛЕМЕНТОВ ИГРЫ*/ {
                //создадим отдельную локальну структуру для размещения элемента
                  allocatedElements = new ElementBitPosition[MapHeight, MapWidth];

                //расположение фрукта
                  CustomPoint = new Point (2, 0);
                  if (!AllocateElement(ElementID.Fruit, MaxFruitCnt,fruits,  ref fruitCnt,FruitColor, onePointMap)) { MessageBox.Show("Не удаётся разместить фрукты на карте. Измените параметры"); return false; }

                //расположение змейки
                  CustomPoint = new Point (0, 0);
                  if (!AllocateElement(ElementID.Snake, 1,          snakeArr,ref score,   SnakeColor, onePointMap)) { MessageBox.Show("Не удаётся разместить змейку на карте. Измените параметры"); return false; }
                  score = 0;
                  headX = snakeArr[0].Location.X / brickSize;
                  headY = snakeArr[0].Location.Y / brickSize;

                  #if TEST 
                  //отобразить номер первого элемента массива snakeArr
                    snakeArr[0].Text = score.ToString();
                    snakeArr[0].Font = new Font ("Arial", 6);
                    snakeArr[0].BringToFront();
                  #endif

                  /*создание метки указателя на хвост*/ {
                    #if TEST
                      lbl_t ??= new Label { 
                          Name = "lbl_t",
                          Size=testLabelSize,
                          Text = "0",
                          AutoSize = false,
                          TextAlign = ContentAlignment.MiddleCenter,
                          Location = new Point (snakeArr[0].Location.X, snakeArr[0].Location.Y+snakeArr[0].Size.Height-testLabelSize.Height)
                      };
                      lbl_t.Font = new Font("Arial", 6);
                      form.Controls.Add(lbl_t);
                      form.Controls["lbl_t"].BringToFront();
                    #endif
                  }
              }

              //form.timer.Interval = SnakeSpeed;
            
              return true;
          }
        /*------------------------------------------------------------------------------------------------------------------*/

        /*------------------------------------------------------------------------------------------------------------------*/
        /*РАЗМЕСТИТЬ ЭЛЕМЕНТ НА КАРТЕ *//*
        -начать счёт кол-ва попыток
        -взять случайную координату
        -проверка возможности размещения всех точек объекта
        >если хоть одна точка не м.б. размещена
          взять другу коорд всего объекта и повторить до кол-ва попыток
        >если все точки могут быть размещены
          -разместить каждую точку объекта
            создать элемент управления
            занести eго в массив элементов соответствующего типа
            добавить в массив карты размещение объекта
        -----------------------------------------------------------------------------------*/ 
          private bool AllocateElement(ElementID element, int cnt, UsedDrawControl[] arr, ref int arr_len, Color color, int[,] map) { 
            //служебные переменные
              Point loc_obj;              //лев верх коорд всего объекта
				      Point loc_i = new Point();  //коорд очередной его точки
				      Random r = new Random();
              ElementsAtXY elementsAtXY=new();
            //определить размерности массива, задающего макет элемента
				      int map_height = map.GetUpperBound(0);
				      int map_width  = map.GetUpperBound(1);

				    //цикл по кол-ву создаваемых элементов
				      for (int i = 0; i < cnt; i++) {
						      int  ok_obj=0;	  //счётчик неудачного размещения
						      bool ok_i;        //успешное размещение очередной точки объекта
						    
               NextObjLocation: 
							  //случайная координата лев. верхнего угла размещаемого объекта
							    #if TEST
                    loc_obj = CustomPoint;
                  #else
                    loc_obj = new Point(r.Next(0, currMapWidth - 1 - map_width), r.Next(0, currMapHeight - 1 - map_height));
                  #endif
                    
                //проверка возможности размещения всего объекта в данном месте
								  for (int y = 0; y <= map_height; y++) {
										  for (int x = 0; x <= map_width; x++) {
											  //если точка не отображается, взять следующую координату
												  if (map[y, x] == 0) continue;     //ask ошибка при F11
                        //координата очередной размещаемой точки
												  loc_i.X = loc_obj.X + x;
												  loc_i.Y = loc_obj.Y + y;
											  //проверка, разрешено ли резмещение точки в этом месте
                          elementsAtXY.Read(loc_i);
												  ok_i = elementsAtXY.IsOverlapAllowed(element);
												  if (!ok_i ) {
                              ok_obj += 1;            
						                //если все попытки вышли - неуспешный выход 
						                  if (ok_obj == 20) return false;
                              goto NextObjLocation;	  //взять брать слудующую начальную координату всего объекта
                          }
										  }
								  }
                //размещение объекта в данном месте
								  for (int y = 0; y <= map_height; y++) {
										  for (int x = 0; x <= map_width; x++) {
											  //если точка не отображается, взять следующую координату
												  if (map[y, x] == 0) continue;    

                        //размещение элемента в массиве карты
                          allocatedElements[loc_obj.Y + y, loc_obj.X + x] |= (ElementBitPosition)Math.Pow(2,(int)element);

											  //разместить элемент в соотв массиве и на экране
											    //координаты на экране
                          loc_i.X = (loc_obj.X + x) * BrickSize;
												  loc_i.Y = (loc_obj.Y + y) * BrickSize;

                          arr[arr_len] = new UsedDrawControl {
												    Location = loc_i,
												    Size = new Size(BrickSize, BrickSize),
												    BackColor = color,
                            TextAlign = ContentAlignment.TopLeft,
                          };
												  form.Controls.Add(arr[arr_len]);
												  arr_len += 1;
										  }
								  }
				      }
				     return true;
			    }
          private Point CustomPoint;
        /*------------------------------------------------------------------------------------------------------------------*/

        //Пуск змейки
          public void Start() {
              //чтение новых значений параметров с формы
              TakeValue("MapWidth",  ref mapWidth);
              TakeValue("MapHeight", ref mapHeight);
              TakeValue("BrickSize", ref brickSize);

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
          
    }




}
