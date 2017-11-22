/************************ Kütüphane Dosyaları ************************************/
#include <LiquidCrystal.h>
#include <dht.h>

/************************ Sensor Pinleri ************************************/
#define     GAZPIN						A10     // Gaz Sensörü pini
#define		BUZZER 						42		// Buzzer Pin
#define 	LED						 	43		// LED pin
#define 	DHT11PIN 					44		// Sicaklik ve Nem sensoru pini

/***************************GAZ id leri*****************************/
#define         g_lpg                   1
#define         g_CO          			3
#define         g_duman                 5

/************************ Tanımlar ************************************/
LiquidCrystal lcd(8, 9, 4, 5, 6, 7);
dht DHT;

/********************************************************/
float           Ro = 10;                            // Ro giriş değeri 10 kilo ohm
float 			gazDegeri;							// Sensörden okunan değer

int d_lpg;
int d_CO;
int d_duman;


void setup() {
  Serial.begin(9600);								// Serial ekranı 9600 bps hızında başlattık
  Serial1.begin(9600);								// Serial ekranı 9600 bps hızında başlattık Bu serial Bluetooh için.
  lcd.begin(16, 2);									// Ekrani baslattik.
  lcd.setCursor(0, 0);								//Imleci 0.0 noktasina getirdik.



  lcd.print(" ORTAM  ANALIZI");
  lcd.setCursor(0, 1);
  lcd.print("  YAPILIYOR...");


  Ro = SensorAyarla(GAZPIN);                       //Sensoru ayarlama komutu.

  pinMode(BUZZER, OUTPUT);
  pinMode(LED, OUTPUT);
  noTone(BUZZER);



}

void loop() {
	gazDegeri	= analogRead(GAZPIN);									//Gaz sensorunden verileri cektik.
	int chk = DHT.read11(DHT11PIN);										// Sıcaklık sensorunden verileri cektik

	d_lpg 			= GazDegerleriniAl(SensorOku(GAZPIN)/Ro,g_lpg);
	d_CO 			= GazDegerleriniAl(SensorOku(GAZPIN)/Ro,g_CO);
	d_duman 		= GazDegerleriniAl(SensorOku(GAZPIN)/Ro,g_duman);
	Alarm(d_lpg, d_CO, d_duman);

//----------- Display e basılır. ---------------//
	lcd.setCursor(0,0);
	lcd.print(" SIC|DUM|LPG|CO ");             //Arduino nun Displayinde basılacak birinci satır


	//Sıcaklık - Nem
	if (DHT.temperature != (float)-999.00){    // DHT11 Sıcaklık Sensörünün hata durumunda -999 değerini ekrana basmaması için optimizasyo yapılmıştır.

		lcd.setCursor(0,1);
		lcd.print("     ");
		lcd.setCursor(1,1);
		lcd.print((int)DHT.temperature);
		lcd.print("C");
	}

	//Duman
	lcd.setCursor(5,1);
	lcd.print("    ");
	lcd.setCursor(5,1);
	lcd.print(d_duman);

	//LPG
	lcd.setCursor(9,1);
	lcd.print("    ");
	lcd.setCursor(9,1);
	lcd.print(d_lpg);

	//KARBON MONOKSİT
	lcd.setCursor(13,1);
	lcd.print("   ");
	lcd.setCursor(13,1);
	lcd.print(d_CO);

	if ((int)DHT.temperature > 30){
			lcd.setCursor(0,1);
			lcd.print("!");
	}
	if (d_duman > 100){
			lcd.setCursor(4,1);
			lcd.print("!");
	}
	if (d_lpg > 23){
		lcd.setCursor(8,1);
		lcd.print("!");
	}
	if (d_CO > 30){
			lcd.setCursor(12,1);
			lcd.print("!");
	}
//----------- Serial ekrana basılır. ---------------//
   Serial.print((int)DHT.temperature);
   Serial.print(",");
   Serial.print(d_lpg);
   Serial.print(",");
   Serial.println(d_CO );

   Serial1.print((int)DHT.temperature);
   Serial1.print(",");
   Serial1.print(d_lpg);
   Serial1.print(",");
   Serial1.println(d_CO );

   //Serial.print("---------------------------------------------");
}

/***************************** SensorAyarla ****************************************
Girdi:   mq_pin - analog pin
Cikti:  Sensorun Ro Degeri
************************************************************************************/
float SensorAyarla(int gaz_pin){
  int i;
  float RS_degeri=0,r0;
  int okunan;

  for (i=0;i<50;i++) {                     				// Sensör Değerini 50 defa oku
	okunan = analogRead(gaz_pin);
    RS_degeri += (float)1*(1023-okunan)/okunan;
    delay(500);
  }
  RS_degeri = RS_degeri/50;              				// 50 defa okunan değerin ortalamasını al

  r0 = RS_degeri/4.434;                      			// Temiz havadaki sensör direnci

  return r0;
}

/*****************************  SensorOku *********************************************
Girdi:   gaz_pin - analog pin
Cikti:  Sensorun Rs degeri
************************************************************************************/
float SensorOku(int gaz_pin){
  int i;
  float rs=0;
  int okunan;

  for (i=0;i<5;i++) {
	okunan = analogRead(gaz_pin);
    rs += (float)1*(1023-okunan)/okunan;
    delay(50);
  }

  rs = rs/5;

  return rs;
}

/*****************************  GazDegerleriniAl **********************************
Girdi:   rs_ro_orani - Rs / Ro
         gaz_id      - Gaz tipi
Cikti:  Hedeflenen gazin ppm degeri
************************************************************************************/
int GazDegerleriniAl(float rs_ro_orani, int gaz_id){
  if ( gaz_id == g_lpg ) {
    return (pow(10,((-3.123*(log10(rs_ro_orani))) + 3.565))); // bu oran gelen gazın LPG olduğunu gösterir.
  } else if ( gaz_id == g_CO ) {
    return (pow(10,((-19.54*(log10(rs_ro_orani))) + 14.5))); // Bu oran gelen gazın Karbon monoksit olduğunu gösterir
  } else if ( gaz_id == g_duman ) {
    return (pow(10,((-9.016*(log10(rs_ro_orani))) + 7.823))); // Bu oran gelen gazın duman olduğunu gösterir.
  }
  return 0;
}

void Alarm(int lpg, int co, int duman){
	if (lpg >23 || co > 30 || duman > 100) {
		digitalWrite(LED, HIGH);
		tone(BUZZER, 440);
	} else {
		digitalWrite(LED, LOW);
		noTone(BUZZER);
	}

}
