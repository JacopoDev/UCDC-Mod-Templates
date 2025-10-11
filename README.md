# Game Modding Templates

This repository contains template mods for **Unity-Chan: Desktop Companion** to help new developers quickly get started with modding.

---

## 📜 Mod List


### 1. Base Translation Mod
_Contains game UI an AI text data in CSV files._<br />
_Can be used to add own text transtlations by replacing values._

---

### 2. Basic Repeater
_Simple example mod that overrides Unity-Chan text generation to repeat one phrase._<br />
_Most basic example of the use of Text-Gen mod interface and Mod Settings._

---

### 3. ChatGPT Mod
_Replaced text generation with API call to OpenAI._<br />
_Requires setting up OpenAI account and generated API key._

---

### 4. ElevenLabs Mod
_Voice synthesis mod using ElevenLabs API._ <br />
_Requires user to set up their ElevenLabs account in mod settings_


---

### 5. Google Cloud Voice Mod
_Integrates Google Cloud's speech services._ <br />
_Requires user to set up their Google Cloud Console account in mod settings_

---

### 6. OpenAI TTS Mod
_Integration of OpenAI TTS API for Unity-chan to speak normal voice<br />
_Has good quality and is cheaper than ElevenLabs_

---

### 7. OpenWebUi Mod
_A mod that connects to OpenWebUI._ <br />
_An alternative way for built-in Text AI generation_

---

### 8. Simplest Skin Mod
_Minimal example for changing a character's skin._  
_More advanced skin mods templates are in a separate repository_  
👉 [Template UCDC VRM Skin Mods](https://github.com/JacopoDev/UCDC-VRM-Mod-Templates)

---

### 9. Template Mod
_A generic mod template for starting from scratch._<br />
_Shows how to set up mod settings_

---

### 10. Template Scene Mod
_Basic mod showcasing how to add a scene that can be accessed by clicking on the door in Kohaku's Room_<br />
_Scene contains necessary object items for Unity-chan and camera placement and simple environment_

---

### 11. Uwu Prompt Mod
_A cursed mod that makes Unity-chan speak awfully sweet. uwu >⩊<_<br />
_An example how you can override the AI system prompt_

---

## 🛠 How to Use These Templates
1. Clone/download this repository
2. Open the project in Unity Engine
3. Select the mod to build by clicking **Mod Tools -> Exporter** and mod name in dropdown
4. Build and load it into the game.
5. Click **Build Mod!**
6. Move newly created `*.ucdcmod` file to `%USERPROFILE%/unityChanCompanion/Mods/` directory
---

## 📄 License
This toolset is provided under the [MIT License](LICENSE).  
You are free to use, modify, and distribute it.  
No warranty is provided.
