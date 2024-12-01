# Applications

## Subapplication 1

Det er en del ting som gjenstår før vi er ferdige med denne delen. Først og fremst må vi fikse dette med database og bruker-funksjonalitet.

Det som gjenstår der er:

- [x] Legge inn Owner-Id felt på producer
- [x] Legge inn en Create Producer side slik at en bruker kan opprette producer med beskrivelse, logo og slikt. Her må også OwnerId-feltet bli henta fra brukeren som oppretter.
- [x] Legge inn Owner-Id felt på product
- [x] Fjern "New Products" og "My Products" når man ikke er logga inn

Andre ting som gjenstår:

- [ ] Category-sidene bør være basert på query params (se denne: https://stackoverflow.com/questions/41577376/how-to-read-values-from-the-querystring-with-asp-net-core)
- [ ] Legg inn bedre data inn i Databasen
- [ ] Flytt søkefeltet under tittel eller kategorier
- [ ] NutriScore i databasen
- [ ] Unit-testing
- [ ] Gå gjennom og forbedre kommentarer i koden
- [ ] Fikse read-more page
- [ ] Legg til felter som beskrivelse, tittel og logoUrl for producer. Disse må komme frem på produktsidene på et vis.

## Subapplication 2

- [ ] Migrate all pages to react-components
- [x] Add authorization
- [ ] Handle Image-upload
- [x] Make all the endpoints in the API
- [ ] Add frontend field-validation
- [ ] Add frontend-error handling
- [x] Connect the react-application to use the API
- [ ] Go through the report
- [ ] Make sure the deliverables are correct
