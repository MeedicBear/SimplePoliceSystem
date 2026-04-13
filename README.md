# **SimplePoliceJail**  
*A modular Police, Jail, Inmate Job, and Impound System for Unturned (SDG Rocket)*  

![License](https://img.shields.io/badge/License-MIT-blue.svg)
![Unturned](https://img.shields.io/badge/Unturned-SDG%20Rocket-green.svg)
![Status](https://img.shields.io/badge/Status-Active-success.svg)
![Language](https://img.shields.io/badge/Language-C%23-239120.svg)

---

## 📌 **Overview**

**SimplePoliceJail** is a complete law‑enforcement and incarceration framework for Unturned servers using **SDG’s built‑in Rocket**.  
It provides police tools, a jail system, inmate jobs, a vehicle impound system, and a fully configurable job engine.

Designed for **RP**, **semi‑RP**, and **law‑enforcement‑focused** servers.

---

## ✨ **Features**

### 👮 Police Tools
- Arrest players with a timer  
- Release jailed players  
- Handcuff & uncuff (restricts movement + actions)  
- Impound vehicles  
- Configurable permissions  

### 🔒 Jail System
- Configurable jail & release positions  
- Optional movement freeze  
- Automatic release when timer ends  
- `/jailtime` command for inmates  

### 🧹 Inmate Job System
Supports **multiple job types**, each fully configurable:

| Job Type | Description |
|----------|-------------|
| **ZONE** | Player enters a zone to complete a task |
| **ITEM** | Player must hold a specific item |
| **DELIVERY** | Player delivers an item to a drop‑off zone |

Each job can define:

- XP reward  
- Money reward  
- Jail time reduction  
- Cooldown  
- Zones  
- Required item  
- Delivery target  

### 🚓 Vehicle Impound System
- Police can impound vehicles  
- Players can reclaim vehicles for a fee  
- Configurable impound location  
- Supports all Rocket economy providers

---

## 🛠 **Installation**

1. Build the project in Visual Studio (`Release` mode).  
2. Copy the compiled DLL from:

```
/SimplePoliceJail/bin/Release/SimplePoliceJail.dll
```

3. Place it into your server’s Rocket plugin folder:

```
Unturned/Servers/<ServerName>/Rocket/Plugins/SimplePoliceJail/
```

4. Start the server once to generate the config.  
5. Edit `Config.xml` to match your jail, job, and impound settings.  
6. Restart the server.

---

## ⚙ **Configuration**

Example snippet:

```xml
<JailConfig>
    <FreezeMovement>true</FreezeMovement>

    <ImpoundFee>250</ImpoundFee>

    <JailPosition x="0" y="0" z="0" />
    <JailYaw>0</JailYaw>

    <ReleasePosition x="10" y="0" z="10" />
    <ReleaseYaw>180</ReleaseYaw>

    <ImpoundLocation x="20" y="0" z="20" />
    <ImpoundYaw>0</ImpoundYaw>
</JailConfig>
```

Jobs are defined in the config class and can be expanded freely.

---

## 🧩 **Commands**

### Police Commands
```
/arrest <player> <seconds>
/release <player>
/cuff <player>
/uncuff <player>
/impound
```

### Player Commands
```
/job
/jailtime
/reclaim
```

---

## 🔐 **Permissions**

| Permission | Description |
|-----------|-------------|
| `policejail.police` | Allows use of arrest, release, cuff, uncuff, impound |

Players without this permission can still use:

- `/job`  
- `/jailtime`  
- `/reclaim`  

---

## 🧪 **Development**

- Target Framework: **.NET Framework 4.8**  
- Compatible with **SDG Rocket (2024+)**  
- Supports **Uconomy** and all Rocket economy providers  
- Modular job engine for easy expansion  

---

## 🚀 **Roadmap**

Potential future features:

- Contraband smuggling jobs  
- Crime point / wanted system  
- Impound auctions  

---

## ❤️ **Credits**

Created by **MeedicBear**.
Designed for premium RP and law‑enforcement gameplay.
