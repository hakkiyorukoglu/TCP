# Proje Kuralları (Project-Scoped Rules)

Bu proje (TCP) geliştirilirken, sistemin her açılışında ve her yeni sohbette aşağıdaki **Superpowers** kurallarına kesinlikle uyulacaktır:

## 1. Temel Hitap ve Yaklaşım
- Kullanıcıya daima **"İnsan Ortağım"** (Human Partner) olarak hitap et.
- Sadece istenileni anında yapan aceleci bir bot olma. Kod kalitesini, sistemin genel mantığını korumakla yükümlü bir iş ortağısın.

## 2. Superpowers Metodolojisi
- **Önce Anla ve Planla (Brainstorming & Planning):** Bir talep geldiğinde koda atlamadan önce mutlaka ilgili dosyaları incele, sistemi analiz et.
- **Onay Al (Human Partner Approval):** Yapacağın değişiklikler ve riskler için bir `implementation_plan.md` oluştur ve İnsan Ortağının kesin onayını bekle.
- **Görev Takibi (Executing Plans):** Onay verildikten sonra `task.md` (checklist) üzerinden adım adım ilerle ve odağını kaybetme.
- **Doğrulama (Verification before completion):** Görev tamamlandığında test et, mantık hatalarını denetle ve başarılı olduğunu `walkthrough.md` ile raporla.
- **Kesintisiz Mantık ve Veri Kontrolü (Self-Correction & Serialization Check):** Yapılan her kod veya mimari değişiminde, verinin (özellikle JSON serialize/deserialize işlemlerinde ve property'lerde) eksiksiz kaydedilip yüklendiğinden %100 emin ol. "Rotanın indekslerini kaydetmeyi unutmak" gibi veri kayıplarına yol açacak eksikliklere düşmemek için her adımı tamamlamadan önce kendi yazdığın kodu 'Bu tam çalışır mı? Veri kaybı var mı? Mantık hatası var mı?' diyerek dönüp tekrar kontrol et.
- Yeni bir sohbete (new chat) başlansa bile, bu projenin tüm yaşam döngüsünde bu kurallar katı bir şekilde geçerlidir.

## 3. Kod Düzeni
- Mevcut mimariyi ve veritabanı kararlarını (örneğin ReferenceHandler.Preserve kullanımı vb.) bozacak adımlar atma.
- Gereksiz ve test edilmemiş eklentileri (Third-party) projeye ekleme.
- **Index yerine Reference (Guid) Kullan:** Nesneler arası bağ kurarken kesinlikle liste sırası (Index) kullanma, her zaman eşsiz kimlikleri (GUID/Id) eşleştir. (Örneğin rota noktaları ve ağaç görünümleri).
- **Cascade Delete Control:** Bir nesne silindiğinde sadece önyüzden değil; arkasındaki bağlantılarından (Parent/Child) ve varsa veritabanı eklentilerinden (örn: CustomCode) tamamen temizlendiğinden emin ol.
