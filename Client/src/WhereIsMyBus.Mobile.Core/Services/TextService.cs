using WhereIsMyBus.Mobile.Core.Interfaces;
namespace WhereIsMyBus.Mobile.Core.Services;
public sealed class TextService(IUserPreferences preferences) : ITextService
{
    private static readonly Dictionary<string,string[]> Texts=new()
    {
        ["FindBus"]=["Find your bus","మీ బస్సును కనుగొనండి","अपनी बस खोजें"],
        ["Subtitle"]=["Travel with confidence","నమ్మకంగా ప్రయాణించండి","भरोसे के साथ यात्रा करें"],
        ["BusNumber"]=["Bus registration number","బస్సు రిజిస్ట్రేషన్ నంబర్","बस पंजीकरण नंबर"],
        ["Search"]=["Search bus","బస్సును వెతకండి","बस खोजें"],
        ["From"]=["From","నుండి","से"],["To"]=["To","వరకు","तक"],
        ["FindRoute"]=["Find buses on route","మార్గంలో బస్సులను వెతకండి","मार्ग पर बसें खोजें"],
        ["Empty"]=["No buses found. Try another search.","బస్సులు దొరకలేదు. మరలా వెతకండి.","कोई बस नहीं मिली। फिर खोजें।"],
        ["EnterNumber"]=["Enter a valid bus number, for example TS09AB1234.","సరైన బస్సు నంబర్ నమోదు చేయండి. ఉదా TS09AB1234.","सही बस नंबर दर्ज करें, जैसे TS09AB1234।"],
        ["SelectRoute"]=["Choose different From and To locations.","వేర్వేరు బయలుదేరు మరియు చేరే ప్రాంతాలను ఎంచుకోండి.","अलग प्रस्थान और गंतव्य स्थान चुनें।"],
        ["Offline"]=["You are offline. Connect to the internet and try again.","ఇంటర్నెట్ కనెక్షన్ లేదు. కనెక్ట్ చేసి మరలా ప్రయత్నించండి.","इंटरनेट नहीं है। जुड़कर फिर कोशिश करें।"],
        ["Error"]=["Could not reach the service. Please try again.","సేవ అందుబాటులో లేదు. మరలా ప్రయత్నించండి.","सेवा उपलब्ध नहीं है। फिर कोशिश करें।"],
        ["Loading"]=["Loading…","లోడ్ అవుతోంది…","लोड हो रहा है…"],
        ["Unavailable"]=["Tracking unavailable","ట్రాకింగ్ అందుబాటులో లేదు","ट्रैकिंग उपलब्ध नहीं है"],
        ["Passenger"]=["Passenger","ప్రయాణికుడు","यात्री"],
        ["Driver"]=["Driver","డ్రైవర్","चालक"],
        ["DriverNote"]=["Driver sign-in and location sharing will be available in the next milestone.","తదుపరి దశలో డ్రైవర్ లాగిన్ మరియు లొకేషన్ షేరింగ్ అందుబాటులోకి వస్తాయి.","अगले चरण में चालक लॉगिन और स्थान साझा करना उपलब्ध होगा।"],
        ["Retry"]=["Load locations","ప్రాంతాలను లోడ్ చేయండి","स्थान लोड करें"],
        ["StartHint"]=["Search by number or choose your route.","నంబర్ లేదా మార్గంతో వెతకండి.","नंबर या मार्ग से खोजें।"]
    };
    public string Language { get; private set; }=Normalize(preferences.Language);
    public string this[string key] => Texts.TryGetValue(key,out var values) ? values[Language=="te" ? 1 : Language=="hi" ? 2 : 0] : key;
    public void SetLanguage(string language) { Language=Normalize(language); preferences.Language=Language; }
    private static string Normalize(string? language) => language is "te" or "hi" ? language : "en";
}
