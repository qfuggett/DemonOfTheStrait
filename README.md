# 🎮 Threshold

*A time simulation horror & survival game built in Unity.*

Inspired by **Sinners** and **cozy games**: a group of happy-go-lucky birthday party guests must survive the night while being hunted by a gang of blood-thirsty vampires.  

Your goal? Keep all the partiers alive until dawn — while the vampires try to lure them outside. 🎂🧛🏽‍♀️

---
## 🛠️ Project Setup

### 🌐 1. Install Unity Hub + Unity Editor
- Download [Unity Hub](https://unity.com/download)
- Within the Hub, install **Unity 6.2**
- Add the **Web Build Module** during install

---

### :octocat: 2. Prep Your Machine for Git

#### Install Git or Github Desktop
[Git](https://git-scm.com/downloads)

  OR
  
[GitHub Desktop](https://desktop.github.com) *(installs Git automatically)*

---

### 🔐 3. Set Up SSH Authentication

_Using **SSH** helps to avoid typing passwords every time you push code._

#### ✅ Step 3a: Check if you already have an SSH key

`ls ~/.ssh`. 

_In most cases, only need one ssh key per machine._ 

- If you return:
`id_ed25519 and id_ed25519.pub (or id_rsa)`
you already have an ssh key! **Proceed to step 4.** 

#### ✅ Step 3b: Check if you already have an SSH key
_Since nothing was returned, you have to create an ssh key:_<br/>
- In your terminal, create the key: 

`ssh-keygen -t ed25519 -C "your_email@example.com"`

_The email is what you used to sign up with your Github account._

- Start the SSH agent and add the key to your machine: 

`eval "$(ssh-agent -s)"
ssh-add ~/.ssh/id_ed25519`

- Retrieve and copy your key:

`cat ~/.ssh/id_ed25519.pub`

- Add your ssh key <a href="https://github.com/settings/ssh/new">here</a>.

- Paste the key, keep the **Key Type** as "Authentication Key" and **Title** it (typically your machine name or its use-identity).

---

### :arrow_down: 4. Clone the project

#### ⬛ Clone via Terminal

- Open terminal and create a folder to store your project:

`mkdir XboxGameCamp`

- Clone the repository using the ssh method: 

`git clone git@github.com:qfuggett/Threshold.git`

#### 🖥️ Clone via Github desktop

- Open GitHub Desktop → File → Clone Repository
- Paste this SSH URL: `git@github.com:qfuggett/Threshold.git`

**OR**

- Navigate to the project on Github, select "Code" and select "Open With Github Desktop"

---

### 🌐 5. Open project in Unity Hub

- Click 'Locate' and find and select the "Threshold" project to open.
<br/>

**<div align="center">🧛🏽Velcome to the project!!😱</div>**

---

## <div align="center">🧠 Important Things to Remember (Team Git Workflow)</div>

_<div align="center">Stay synced. Stay safe. No one likes code possessed by merge conflicts. 🧟‍♂️</div>_

### ✅ Always Pull Before You Work
Before you make any changes, pull the latest version of the repo:

`git pull origin main`

### 🪄 Create a New Branch Before Making Changes
_Never work directly on the main branch._

Instead, create your own branch for features, fixes, or experiments:

`git checkout -b feature/your-branch-name`

### ✅ Always merge all new changes when checked into your own branch

```
git pull origin main
git merge main
```

### 💾 Stage & Commit Your Changes
After you’ve made changes:

```
git add .
git commit -m "Short, clear message about what you did"
```

### 🚀 Push Your Branch to GitHub
`git push`
