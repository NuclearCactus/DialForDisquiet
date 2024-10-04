#include "pitches.h"
#include <KeyMatrix.h>
#include <Bounce2.h>

// -- Melody Stuff --
int melodyA[] = { 200, NOTE_C4, 4, NOTE_G3, 8, NOTE_G3, 8, NOTE_A3, 4, NOTE_G3, 4, SILENCE, 4, SILENCE, 8, NOTE_B3, 4, NOTE_C4, 4, SILENCE, 4 };
int melodyB[] = { 108, NOTE_E4, 2, NOTE_G4,4, NOTE_D4,2, NOTE_C4,8, NOTE_D4,8, NOTE_E4,2, NOTE_G4,4, NOTE_D4,-2, NOTE_E4,2, NOTE_G4,4, NOTE_D5,2,
  NOTE_C5,4, NOTE_G4,2, NOTE_F4,8, NOTE_E4,8, NOTE_D4,-2, NOTE_E4,2, NOTE_G4,4, NOTE_D4,2, NOTE_C4,8, NOTE_D4,8, NOTE_E4,2, NOTE_G4,4, NOTE_D4,-2,
  NOTE_E4,2, NOTE_G4,4, NOTE_D5,2, NOTE_C5,4, NOTE_G4,2, NOTE_F4,8, NOTE_E4,8, NOTE_F4,8, NOTE_E4,8, NOTE_C4,2, NOTE_F4,2, NOTE_E4,8, NOTE_D4,8,
  NOTE_E4,8, NOTE_D4,8, NOTE_A3,2, NOTE_G4,2, NOTE_F4,8, NOTE_E4,8, NOTE_F4,8, NOTE_E4,8, NOTE_C4,4, NOTE_F4,4, NOTE_C5,-2 };
int melodyC[] = { 180,  NOTE_E5, 8, NOTE_D5, 8, NOTE_FS4, 4, NOTE_GS4, 4, NOTE_CS5, 8, NOTE_B4, 8, NOTE_D4, 4, NOTE_E4, 4, 
  NOTE_B4, 8, NOTE_A4, 8, NOTE_CS4, 4, NOTE_E4, 4, NOTE_A4, 2 };
int melodyD[] = { 105, NOTE_B4, 16, NOTE_B5, 16, NOTE_FS5, 16, NOTE_DS5, 16, NOTE_B5, 32, NOTE_FS5, -16, NOTE_DS5, 8, NOTE_C5, 16,
  NOTE_C6, 16, NOTE_G6, 16, NOTE_E6, 16, NOTE_C6, 32, NOTE_G6, -16, NOTE_E6, 8, NOTE_B4, 16,  NOTE_B5, 16,  NOTE_FS5, 16, 
  NOTE_DS5, 16,  NOTE_B5, 32,  NOTE_FS5, -16, NOTE_DS5, 8,  NOTE_DS5, 32, NOTE_E5, 32,  NOTE_F5, 32,
  NOTE_F5, 32,  NOTE_FS5, 32,  NOTE_G5, 32,  NOTE_G5, 32, NOTE_GS5, 32,  NOTE_A5, 16, NOTE_B5, 8 };

// Mais melodias aqui: https://github.com/robsoncouto/arduino-songs/tree/master


int *melody;
int numNotes = -1;

uint8_t buzzerPin = 8;

// -- Buttons stuff --
KEY_MATRIX_PHONE(keypad, 4, 5, 6, 7, 12, 9, 10);
Bounce2::Button receiver = Bounce2::Button();
uint8_t receiverPin = 2;

uint8_t ring = -1;
bool receiverUp = false;
bool upInterrupt = false;

void setup() {
  Serial.begin(9600);

  receiver.attach( receiverPin, INPUT_PULLUP ); // USE INTERNAL PULL-UP
  receiver.interval(5); // DEBOUNCE INTERVAL IN MILLISECONDS
  receiver.setPressedState(LOW); // INDICATE THAT THE LOW STATE CORRESPONDS TO PHYSICALLY PRESSING THE BUTTON

}

void loop() {

  // --- Receber inputs ---
  if(Serial.available() > 0)
  {
    String inStr = Serial.readString();
    inStr.trim();

    if(!receiverUp)
    {
      if(inStr == "ringA")
      {
        melody = melodyA;
        numNotes = (sizeof(melodyA) - sizeof(int)) / sizeof(int) / 2;
      }
      else if(inStr == "ringB")
      {
        melody = melodyB;
        numNotes = (sizeof(melodyB) - sizeof(int)) / sizeof(int) / 2;
      }
      else if(inStr == "ringC")
      { 
        melody = melodyC;
        numNotes = (sizeof(melodyC) - sizeof(int)) / sizeof(int) / 2;
      }
      else if(inStr = "ringD")
      {
        melody = melodyD;
        numNotes = (sizeof(melodyD) - sizeof(int)) / sizeof(int) / 2;
      }
    }


      while(Serial.available())
        Serial.read();
  }

  // --- Ring Ring ---
  if(numNotes != -1)
    Ring();

  // --- KeyMatrix ---
  if(receiverUp)
    ReadKeys();

  // --- Receiver ---
  receiver.update();
  if(receiver.released() || upInterrupt) // Levantou
  {
    receiverUp = true;
    upInterrupt = false;
    numNotes = -1;
    Serial.println("up");
  }
  else if(receiver.pressed()) // Pousou
  {
    receiverUp = false;
    Serial.println("down");
  }


  delay(10);
}

void ReadKeys()
{  
  if (keypad.pollEvent() && keypad.event.type == KM_KEYDOWN) 
    Serial.println(keypad.event.c);
}

void Ring()
{
  int divider, noteDuration;

  int tempo = melody[0];
  int wholenote = (60000 * 4) / tempo;

  for (int thisNote = 1; thisNote < numNotes * 2; thisNote += 2) {

    // calculates the duration of each note
    divider = melody[thisNote + 1];
    if (divider > 0) {
      // regular note, just proceed
      noteDuration = (wholenote) / divider;
    } else if (divider < 0) {
      // dotted notes are represented with negative durations!!
      noteDuration = (wholenote) / abs(divider);
      noteDuration *= 1.5; // increases the duration in half for dotted notes
    }

    // we only play the note for 90% of the duration, leaving 10% as a pause
    tone(buzzerPin, melody[thisNote], noteDuration*0.9);

    // Wait for the specief duration before playing the next note.
    delay(noteDuration);
    
    // stop the waveform generation before the next note.
    noTone(buzzerPin);

    receiver.update();
    if(receiver.released()) // Levantou
    {
      upInterrupt = true;
      return;
    }
  }
}
