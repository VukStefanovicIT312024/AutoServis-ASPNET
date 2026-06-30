import { useEffect, useState } from "react";
import { Alert, Button, Card, Container, Spinner } from "react-bootstrap";
import { Link } from "react-router-dom";

const WEATHER_API_URL =
  "https://api.open-meteo.com/v1/forecast?latitude=45.2671&longitude=19.8335&current=temperature_2m,wind_speed_10m,weather_code&timezone=Europe%2FBelgrade";

const weatherDescriptions = {
  0: "Vedro",
  1: "Pretežno vedro",
  2: "Delimično oblačno",
  3: "Oblačno",
  45: "Magla",
  48: "Magla sa injem",
  51: "Slaba rosulja",
  53: "Umerena rosulja",
  55: "Jaka rosulja",
  61: "Slaba kiša",
  63: "Umerena kiša",
  65: "Jaka kiša",
  71: "Slab sneg",
  73: "Umeren sneg",
  75: "Jak sneg",
  80: "Slabi pljuskovi",
  81: "Umereni pljuskovi",
  82: "Jaki pljuskovi",
  95: "Grmljavina",
};

function HomePage() {
  const [weather, setWeather] = useState(null);
  const [weatherLoading, setWeatherLoading] = useState(true);
  const [weatherError, setWeatherError] = useState("");

  useEffect(() => {
    let componentIsMounted = true;

    async function fetchWeather() {
      try {
        const response = await fetch(WEATHER_API_URL);

        if (!response.ok) {
          throw new Error("Greška pri učitavanju vremenske prognoze.");
        }

        const data = await response.json();

        if (componentIsMounted) {
          setWeather(data.current);
        }
      } catch (error) {
        if (componentIsMounted) {
          setWeatherError("Vremenska prognoza trenutno nije dostupna.");
        }
      } finally {
        if (componentIsMounted) {
          setWeatherLoading(false);
        }
      }
    }

    fetchWeather();

    return () => {
      componentIsMounted = false;
    };
  }, []);

  return (
    <section className="home-hero">
      <Container>
        <div className="home-hero-content">
          <p className="hero-label">Pouzdan servis za vaše vozilo</p>
          <h1>AutoServis</h1>
          <p>
            Pregledajte dostupne usluge i jednostavno zakažite termin za svoje
            vozilo.
          </p>

          <Button as={Link} to="/usluge" variant="danger" size="lg">
            Pregled usluga
          </Button>

          <Card className="weather-card mt-4">
            <Card.Body>
              <p className="hero-label mb-2">Vremenska prognoza</p>
              <h5 className="mb-3">Novi Sad</h5>

              {weatherLoading && (
                <div className="d-flex align-items-center gap-2">
                  <Spinner animation="border" size="sm" />
                  <span>Učitavanje vremenskih podataka...</span>
                </div>
              )}

              {!weatherLoading && weatherError && (
                <Alert variant="warning" className="mb-0">
                  {weatherError}
                </Alert>
              )}

              {!weatherLoading && weather && (
                <div>
                  <p className="mb-2">
                    <strong>Temperatura:</strong>{" "}
                    {Math.round(weather.temperature_2m)}°C
                  </p>
                  <p className="mb-2">
                    <strong>Vetar:</strong>{" "}
                    {Math.round(weather.wind_speed_10m)} km/h
                  </p>
                  <p className="mb-0">
                    <strong>Uslovi:</strong>{" "}
                    {weatherDescriptions[weather.weather_code] ||
                      "Nema detaljnog opisa"}
                  </p>
                </div>
              )}
            </Card.Body>
          </Card>
        </div>
      </Container>
    </section>
  );
}

export default HomePage;