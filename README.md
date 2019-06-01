## 《C#.Net综合应用程序开发》

### 学院：软件学院  班级：4  学号：3017218151 姓名：代有为 日期：2019年4月18日 

## 功能概述：
  ![Fail](https://github.com/Dai-Youwei/lab1/blob/master/图片1.png);
  
  在1处：combobox显示所有pc机上的串口名，1处combobox显示设定的传输波特率速率(BPS)（9600、19200、38400、57600）。选择该速率时，请与Arduino开发板的串口BaudRate一致。
在2处：点击“连接”按钮连接到开发板，2处点击“关闭”按钮断开连接。
在4处：以ListView显示发送的数据和返回的实时信息。
在5处，显示出Arduino上温度、光强随时间变化的曲线图。在位置8显示出两个数据物理数值，光强度可以用ADC值表示。
在6处：利用滑块(slider)控制Arduino板上的PWM输出端，实现五种LED灯的明暗控制（数值范围0-255），在界面的○处显示RGB混合色,发送按钮将设定的各LED的PWM值以MIDI协议规定的格式发送给Arduino，Arduino可以完成各PWM输出端的PWM数值设定。
在7处：点击log开始按钮，显示FileDialog让用户填写需要记录的文件名，文件名的缺省值：log-YYYY-MM-DD-HH-mm-SS.txt,YYYY为年份、MM为月份、DD为日期、HH为时、mm为分、SS为秒，用户点击OK就开始记录实时信息，点击Cancel放弃本次记录。点击log结束按钮，将记录结果存盘关闭。需要存储的数据为：温度和LED灯PWM数据、串口设定值、实时通讯数据等，存储格式为csv或Json或XML等格式之一。  
## 代码总量：600行

## 工作时间：7天

## 知识点总结图：
![Fail](https://github.com/Dai-Youwei/lab1/blob/master/浅海昌蓝.png);

## 结论：
通过对Serial类、数据绑定、事件触发、MIDI协议、端口控制、字节传输的学习，能够对Arduino开发板进行简单的控制。
