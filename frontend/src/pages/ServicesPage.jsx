import { useEffect, useState } from "react";
import { Alert, Col, Container, Form, Row, Spinner } from "react-bootstrap";
import ServiceCard from "../components/ServiceCard";
import { API_URL } from "../config";

function ServicesPage() {
  const [services, setServices] = useState([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function fetchServices() {
      try {
        setLoading(true);

        const response = await fetch(`${API_URL}/api/services`);

        if (!response.ok) {
          throw new Error("Greška pri učitavanju usluga.");
        }

        const data = await response.json();
        setServices(data);
        setError("");
      } catch (error) {
        setError(error.message);
      } finally {
        setLoading(false);
      }
    }

    fetchServices();
  }, []);

  const filteredServices = services.filter((service) => {
    const searchValue = search.toLowerCase();

    return (
      service.name.toLowerCase().includes(searchValue) ||
      service.category.toLowerCase().includes(searchValue) ||
      service.description.toLowerCase().includes(searchValue)
    );
  });

  return (
    <Container className="py-5">
      <div className="page-heading">
        <p className="section-label">Ponuda servisa</p>
        <h1>Usluge autoservisa</h1>
        <p>Pronađite odgovarajuću uslugu za svoje vozilo.</p>
      </div>

      {loading && (
        <div className="text-center py-5">
          <Spinner animation="border" variant="danger" />
          <p className="mt-3">Učitavanje usluga...</p>
        </div>
      )}

      {error && <Alert variant="danger">{error}</Alert>}

      {!loading && !error && (
        <>
          <Form.Control
            type="search"
            placeholder="Pretražite usluge..."
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            className="service-search mb-4"
          />

          <Row className="g-4">
            {filteredServices.map((service) => (
              <Col key={service._id} sm={12} md={6} lg={4}>
                <ServiceCard service={service} />
              </Col>
            ))}
          </Row>

          {filteredServices.length === 0 && (
            <p className="text-center mt-5">Nema pronađenih usluga.</p>
          )}
        </>
      )}
    </Container>
  );
}

export default ServicesPage;