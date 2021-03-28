# Sokoban

Na potrzebę tego problemu, wykonaliśmy grę na silniku Unity 2019.3.7f1.

Za kod odpowiadał: Kacper Staroń.
Poziomy i grafika: Aleksander Brzykcy, Dawid Mularczyk, Mateusz Łoziński, Piotr Łaba.

# Schemat działania

# Menu Główne

1. Po załadowaniu menu gry, `ModulesLoader.cs` ładuje metadane modułów z folderu `StreamingAssets/Modules`.
2. Każdy moduł ładowany jest do klasy `ModulesManager`, która przechowuje dane modułów i poziomów.
3. `ModulesLoader` również ładuje wszystkie poziomy należące do danego modułu.
4. W międzyczasie, klasa `SavesManager` ładuje wszystkie zapisy gier, które znajdują się w `%USER%/LocalLow/DefaultCompany/Sokoban/sav`.
5. Po wczytaniu wszystkich danych, użytkownik wybiera jedną z 3 opcji: `Play`, `Load Save`, `Level Editor`.

# Po wybraniu Play

1. Użytkownik wybiera jeden z wcześniej załadowanych modułów.
2. Po wybraniu modułu, w zależności od typu modułu, `ModuleMenuButton` podejmuje jedną z dwóch decyzji:

**Gdy moduł jest typu DifficultyBased**
1. `DifficultySelector` pokazuje menu z wszystkimi poziomami trudności.
2. Po kliknięciu na jeden poziom trudności, `DifficultySelector` prosi `ModulesManager` o losowy poziom o danym poziomie trudności.
3. Po otrzymaniu poziomu, `DifficultySelector` wysyła prośbę do klasy `WorldTraversalManager` o załadowanie poziomu.

**Gdy moduł jest typu ContinuousLevels**
1. Klasa `ModulesLoader` generuje menu z wszystkimi poziomami.
2. Gracz widzi przed sobą listę poziomów z danego modułu i wybiera jeden.
3. Po kliknięciu na jeden poziom, `LevelMenuButton` wysyła prośbę do klasy `WorldTraversalManager` o załadowanie poziomu.

**Ładowanie poziomu**
1. `WorldTraversalManager` ładuje poziom MainScene.
2. Po załadowaniu poziomu, `WorldTraversalManager` szuka klasy `LevelGenerator` w poziomie.
3. Po znalezieniu klasy, wysyła prośbę do `LevelGenerator` o załadowanie poziomu i wygenerowanie mapy.
4. `LevelGenerator` generuje mapę z pliku `<id>.map`, przy okazji ustawiając pozycję magazyniera, i dodając przy okazji punkty końcowe do klasy `GoalManager`.
5. Po załadowaniu mapy, `LevelGenerator` informuje klasę `GameManager` o tym, iż załadowano poziom.

**Gameplay loop**
1. Klasa `Player` sprawdza czy gracz chciał się poruszyć, jak tak to wykonuje ruch, przy okazji przesuwając pudełka, jeżeli takie przesunięcie istnieje.
2. Gdy pudełko zostaje przesunięte na punkt końcowy, `Player` informuje klasę `GoalManager` o tym, iż dany punkt końcowy został osiągnięty.
3. Klasa `GameManager` sprawdza, czy gracz wcisnął przycisk pauzy (Escape).
4. Jeżeli gracz nie wcisnął pauzy, `GameManager` sprawdza, czy `GoalManager.HasWon` jest równe `True`.
5. Jeżeli jest równe, `GameManager` wyświetla menu końcowe.

**Gameplay loop: Menu Pauzy**
1. Gracz widzi dwie opcje: `Save` i `Return to menu`
2. Po wciśnięciu `Save`, klasa `PauseMenu` wysyła zapytanie do klasy `SaveManager` o zapis gry.
3. `SaveManager` pobiera dane ID obecnego poziomu, ilości punktów, oraz które kafelki na planszy się różnią i zapisuje je.
4. Gdy gracz jednak wcisnął `Return to menu`, `PauseMenu` wysyła zapytanie do `WorldTraversalManager` aby ten wyładował obecny poziom, i załadował `MainMenu`.

**Gameplay loop: Menu Końcowe**
1. W menu końcowym, gracz ma dwie opcje: `Next Level` oraz `Return to menu`.
2. Po wybraniu `Next Level`, klasa `GameManager` sprawdza obecny typ modułu.
3. Gdy moduł jest typu `DifficultyBased`, `GameManager` prosi `ModulesManager` o losowy poziom o danym poziomie trudności, i każe klasie `LevelGenerator` go załadować.
4. Gdy moduł jest typu `ContinuousLevels`, `GameManager` sprawdza czy wartość `NextLevel` obecnego poziom nie jest pusta.
5. Jeżeli jest pusta, `GameManager` prosi `LevelGenerator` o załadowanie tego samego poziomu.
6. Jeżeli nie jest pusta, `GameManager` prosi `ModulesManager` o dany poziom, i każe klasie `LevelGenerator` go załadować.

# Po wybraniu Load Save

1. Klasa `SaveLoader` generuje menu z ostatnimi zapisami pobranymi od klasy `SaveManager`.
2. Po kliknięciu w zapis, `SaveMenuButton` prosi klasę `SaveManager` o załadowanie poziomu.
3. `SaveManager` każe klasie `WorldTraversalManager` o przeniesienie na główny poziom.
4. Po załadowaniu mapy przez `LevelGenerator`, nakładane są różnice przechowywane w zapisie.

# Po wybraniu Level Editor

1. `WorldTraversalManager` ładuje scene `LevelEditor`.
2. W edytorze poziomów, klasa `Editor` sprawdza obecny kafelek na którym jest gracz, i jeżeli jego kursor jest wcisnięty, kładzie obecny kafelek.
3. Jeżeli gracz wciśnie klawisz `Enter`, `Editor` tymczasowo pakuje poziom, i każe `WorldTraversalManager` go załadować jako poziom, do testowania.
4. Jeżeli gracz w trybie testowania również wciśnie `Enter`, klasa `LevelEditorHelper` prosi `WorldTraversalManager` o ponowne przeniesienie na scenę edytora.
5. Jeżeli gracz wciśnie przycisk `Save`, klasa `Editor` pakuje obecny poziom i go zapisuje w folderze `StreamingAssets/Modules/Z_CustomLevels`

# Użyto

* Unity 2019.3.7f1
* Kenney 1-bit RPG Asset Pack
* Kenney Fonts Asset Pack
* Kenney UI Icons Pack