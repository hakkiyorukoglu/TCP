# TCP — Train Control Platform

[![Version](https://img.shields.io/badge/version-TCP--2.2.0-blue.svg)](docs/CHANGELOG.md)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)

**TCP (Train Control Platform)**, tren kontrol sistemleri, ray yerleşim tasarımı ve donanım kartları yönetimini birleştiren tam donanımlı, profesyonel bir masaüstü (WPF) uygulamasıdır.

---

## 🚀 Öne Çıkan Özellikler (v2.1.0)

Uygulama baştan aşağıya gelişmiş yetenekler ve dinamik bir arayüzle donatıldı:

- 🎛 **Özel Cihaz Yönetimi (Electronics):** "Arduino Mega" gibi şablonlar üzerinden kendi cihazlarınızı (My Devices) yaratın. Her cihaz için `MAC Adresi`, `IP Adresi`, `Port`, `LAN Kablosu` ve `Konum` tanımlayarak kayıt altında tutun.
- 🗺 **Gelişmiş Editör ve Haritalama:** 
  - AutoCAD tarzı grid zemin üzerine çoklu arka plan görselleri (harita veya planlar) yükleyin.
  - Şeffaflık (Opacity) kontrolüyle katmanlı haritalar oluşturun.
  - Resimlerin orijinal çözünürlükleri ve en/boy oranları akıllı bellek yöntemiyle korunur.
  - Kaydettiğiniz özel cihazları Harita/Editör paneline ekleyin ve yerleşimlerini tasarlayın.
  - **Yeni! Katmanlar Paneli:** Photoshop benzeri sağ panel ile haritaya eklenen resimleri ve elektronik cihazları yönetin. Tek tıkla gizleyin (Göz ikonu), kilitleyin (Kilit ikonu) ve seçin.
  - Editör sayfa durumunu **Save Page** ve **Load Page** ile kaydedip geri yükleyin.
- 🌍 **Dinamik Dil Desteği:** Tek tıkla tamamen **İngilizce** ve **Türkçe** dilleri arasında geçiş yapın. Tüm sistem anında çevrilir.
- 🌓 **Tema Motoru:** Akıcı ve estetik bir arayüz ile **Karanlık (Dark)** ve **Aydınlık (Light)** temalar arasında geçiş imkanı.
- ⌨ **Terminal ve Konsol:** Her sayfada görünen, katlanabilir ve uygulamanın arka planında olan biteni gösteren canlı terminal konsolu.
- 🔍 **Akıllı Arama:** Uygulama içinde ne aradığınızı birkaç harfle bulmanızı sağlayan dinamik sonuçlu arama motoru.

---

## 💻 Kurulum ve Çalıştırma

```bash
# Projeyi derleyin
dotnet clean
dotnet build

# Uygulamayı başlatın
dotnet run --project TCP.App
```
*(Not: Windows Defender nedeniyle "Uygulama Denetimi İlkesi" hatası alırsanız `TCP.App.exe` dosyasını `bin\Debug\net8.0-windows` içinden manuel olarak veya **Visual Studio** üzerinden başlatın.)*

---

## 📚 Dokümantasyon

Daha derinlemesine bilgi için [`/docs`](docs/) klasöründeki belgelere göz atabilirsiniz:
- **[CHANGELOG.md](docs/CHANGELOG.md)** - Sürüm geçmişi ve güncellemeler
- **[ARCHITECTURE.md](docs/ARCHITECTURE.md)** - Sistem mimarisi ve tasarım prensipleri
- **[ROADMAP.md](docs/ROADMAP.md)** - Gelecek geliştirme hedefleri
- **[SETUP.md](docs/SETUP.md)** - Geliştirme ortamı kurulum talimatları

---

## 🛠 Kullanılan Teknolojiler

- **Platform:** .NET 8, WPF (Windows Presentation Foundation)
- **Mimari:** MVVM (Model-View-ViewModel), Singleton Servisler
- **Tasarım Sistemi:** WPF-UI, Katmanlı (Token-Based) Tema Mimarisi
- **Veritabanı:** `JSON` tabanlı yerel dosya yönetimi (Persistence)

---

## 📜 Lisans

Bu proje kişisel/özel geliştirme aşamasındadır ve şimdilik lisans bilgisi belirtilmemiştir.
