# TCP — Train Control Platform

[![Version](https://img.shields.io/badge/version-TCP--2.9.2-blue.svg)](docs/CHANGELOG.md)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

**TCP (Train Control Platform)**, tren kontrol sistemleri, ray yerleşim tasarımı ve donanım kartları yönetimini birleştiren tam donanımlı, profesyonel bir masaüstü (WPF) uygulamasıdır.

---

## 🚀 Öne Çıkan Özellikler (v2.9.2)

Uygulama baştan aşağıya gelişmiş yetenekler, dinamik bir arayüz ve uç nokta donanım yönetimiyle donatıldı:

- 🛡️ **Kararlılık ve Veri Bütünlüğü**: Simülasyon gecikmelerinin anında iptal edilebilir yapısı (cancellationToken), arka planda çalışan hayalet (zombie) process'lerin `IDisposable` ile engellenmesi. Kaydedilmeyen kodların sessizce çöpe gitmesini önleyen veri kaybı korumaları.
- 🧬 **C# Scripting & Simülasyon Ağı**: Donanım simülasyonu motoru güncellendi. İstasyonlar ve Ana PC artık entegre bir Ağ Kuyruğu (Network Bus) üzerinden `send()` ve `inMsg()` ile iletişim kurabiliyor.
- 🚦 **Sensör Mantığı ve Neon Çizimler**: Gerçekçi okuma sıçrama engelleme (Debounce) ile kilitlenmeyen RFID yönetimi ve enerji durumuna göre tepki veren pürüzsüz Neon Glow LED simülasyonları.
- 🗄️ **SQLite & EF Core Entegrasyonu**: Tüm ağ, istasyon ve harita tasarımlarını güvenli ve kalıcı olarak kaydetmenizi sağlayan Entity Framework tabanlı veritabanı altyapısı.
- 🧪 **Birim Testleri (xUnit & Moq)**: Veritabanı ve state yönetimi gibi kritik çekirdek parçalar endüstri standartlarındaki birim testleriyle koruma altında.
- 🛤️ **Rota (Spline) Çizimi**: Editör üzerinde serbestçe tren hatları ve rotaları (TrackRoute) çizebilme.
- 🚆 **Simülasyon Modülü**: Editör'de çizdiğiniz harita ve yerleştirdiğiniz donanımların *otomatik olarak* Simülasyon ekranına yüklenmesiyle sağlanan 2D kuşbakışı simülasyon deneyimi.
- 🚇 **Kör Otonom Tren Mimarisi (Blind Autonomous Trains)**: Trenler (Arduino Nano kontrollü) Wi-Fi üzerinden gerçek zamanlı haberleşmez. Ana masaüstü bilgisayardan (Örn: "Tren 1, Masa 10'a git") başlangıç talimatını alıp kör otonom hareket ederler.
- 🏷️ **RFID Sinyalizasyon**: Trenin altında RFID etiketi (Tag) bulunur. Ray ayrım noktalarında (makas girişlerinde) masalara/bölgelere ait RFID Okuyucular bulunur. Tren bir okuyucudan geçtiğinde Ana PC, trenin yönlendirilmesi gereken masaya göre ilgili istasyona (Servo Motor) sinyal gönderip makası açar.
- 📡 **Canlı HTTP Donanım İletişimi**: Editör üzerindeki istasyonlarla asenkron (GET/POST) haberleşme.
- 🎛️ **Uzaktan Röle / Pin Kontrolü**: İstasyonlara ait röleleri (ışık, motor, sensör tetikleyici vs.) canlı olarak uygulamadan yönetebilme.
- 🚀 **Uzaktan OTA Firmware Güncelleme**: İstasyonların yazılımlarını (.bin) masaüstünden kablosuz/LAN üzerinden uzaktan tek tıkla yükleyebilme.
- 🏗️ **Katman (Layer) Sistemi**: Profesyonel düzenleme ve kilitleme seçenekleri.
- 🔗 **Papatya Dizilimi (Daisy-Chain)**: Donanımları art arda bağlayarak otomatik ağ keşfi yapabilme.
  - AutoCAD tarzı grid zemin üzerine çoklu arka plan görselleri (harita veya planlar) yükleyin.
  - Şeffaflık (Opacity) kontrolüyle katmanlı haritalar oluşturun.
  - Kaydettiğiniz özel cihazları Harita/Editör paneline ekleyin ve yerleşimlerini tasarlayın.
  - **Esnek Ağ İplikleri:** Modem, İstasyon ve Parçalar (Components) arasındaki bağlantı hiyerarşisini gösteren elastik kesik çizgiler.
  - **3-Sütunlu Profesyonel Arayüz:** Sol tarafta cihaz paleti, ortada geniş çizim alanı ve sağda katmanlar ağacı ile rahat çalışma imkanı.
  - **Ağaç Yapılı (TreeView) Katmanlar:** Cihazlarınızı konumlarına göre otomatik klasörleyen gelişmiş katman paneli.
- 🌍 **Dinamik Dil Desteği:** Tek tıkla tamamen **İngilizce** ve **Türkçe** dilleri arasında geçiş.
- 🌓 **Tema Motoru:** Akıcı ve estetik bir arayüz ile **Karanlık (Dark)** ve **Aydınlık (Light)** temalar.
- ⌨ **Terminal ve Konsol:** Her sayfada görünen, katlanabilir ve uygulamanın arka planında olan biteni gösteren canlı terminal konsolu.

---

## 💻 Kurulum ve Çalıştırma

```bash
# Projeyi derleyin
dotnet clean
dotnet build

# Uygulamayı başlatın
dotnet run --project TCP.App

# Testleri çalıştırın
dotnet test TCP.Tests
```

---

## 🛠 Kullanılan Teknolojiler

- **Platform:** .NET 8, WPF (Windows Presentation Foundation)
- **Mimari:** MVVM (Model-View-ViewModel), Singleton Servisler
- **ORM & Veritabanı:** Entity Framework Core, SQLite
- **Test:** xUnit, Moq
- **Tasarım Sistemi:** WPF-UI, Katmanlı Tema Mimarisi

---

## 📜 Lisans

Bu proje kişisel/özel geliştirme aşamasındadır ve şimdilik lisans bilgisi belirtilmemiştir.
