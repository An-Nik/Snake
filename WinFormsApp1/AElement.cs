using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsedDrawControl = System.Windows.Forms.Label;         //задать контрол, который будет исп-ся для рисования
using System.Windows.Forms;
using static Snake.Snake;

namespace Snake {
    public abstract class AElement {

      #region ОТКРЫТЫЕ ПАРАМЕТРЫ ИГРЫ
      //=============================
        public int MaxCount { get=> maxCount; set=> maxCount=value; }     //максимальное кол-во элементов на поле
        public Color color;
      #endregion------------------------------------------------------------------------------------------------------------

      #region СЛУЖЕБНЫЕ ПАРАМЕТРЫ ОБЕСПЕЧЕНИЯ РАБОТОСПОСОБНОСТИ
      //=======================================================
        internal readonly ElementId id;
        internal int maxCount;                //максимальное кол-во элементов на поле
        internal int count = 0;               //осталось элементов на поле
        public UsedDrawControl[] bricksArr;   //массив фруктов

        //сообщение об ошибке при неудачном расположении объекта на карте
        internal string errMsg;

        //битовая карта элемента для расположения автоматически (без наворотов)
        internal int[,] elementBitMapAuto = onePointMap;

        #if TEST
        //заданные координаты элемента для тестирования и отладки
          public int[,] testPoints;
          internal int testPt_i = 0;
        #endif
      #endregion------------------------------------------------------------------------------------------------------------


        //конструктор
        internal AElement(ElementId id, string labelName, int maxCount, int maxArrayLenght, Color color) {
            this.id=id;
            this.maxCount = maxCount;
            bricksArr = new UsedDrawControl[maxArrayLenght];    //создать массив под элементы
            this.color = color;
            errMsg = $"Не удаётся разместить элемент {id} на карте. Измените параметры";

          //занести в словарь controlValue имя контролов и начальные значения (для изменения цвета при редактировании)
            Snake.AddControl(labelName, maxCount);
        }

        //добавление дополнительных элементов управления на форму
        public virtual void AddControl() {}

        //прочитать значение сформы
        public virtual void ReadControlVlaue() {}

        //расположение элемента
        public virtual bool AllocateElement() {
            if (!Snake.AllocateElement(id, MaxCount, bricksArr, ref count, color, elementBitMapAuto)) {
                MessageBox.Show(errMsg); 
                return false; 
            }
            return true;
        }

        public virtual void HeadOver() {}
    }
}
