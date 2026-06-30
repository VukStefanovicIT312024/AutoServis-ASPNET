import { useEffect, useState } from "react";
import { Alert, Button, Container, Spinner } from "react-bootstrap";
import { FaArrowLeft, FaCheck, FaClock } from "react-icons/fa";
import { Link, useParams } from "react-router-dom";
import { API_URL } from "../config";

function ServiceDetailsPage() {
  const { id } = useParams();

  const [service, setService] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    async function fetchService() {
      try {
        setLoading(true);

        const response = await fetch(`${API_URL}/api/services/${id}`);

        if (!response.ok) {
          throw new Error("Usluga nije pronađena.");
        }

        const data = await response.json();
        setService(data);
        setError("");
      } catch (error) {
        setError(error.message);
      } finally {
        setLoading(false);
      }
    }

    fetchService();
  }, [id]);

  if (loading) {
    return (
      <Container className="py-5 text-center">
        <Spinner animation="border" variant="danger" />
        <p className="mt-3">Učitavanje usluge...</p>
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="py-5">
        <Alert variant="danger">{error}</Alert>

        <Button as={Link} to="/usluge" variant="dark">
          Nazad na usluge
        </Button>
      </Container>
    );
  }

  return (
    <Container className="py-5">
      <Button
        as={Link}
        to="/usluge"
        variant="link"
        className="px-0 mb-4 text-decoration-none"
      >
        <FaArrowLeft className="me-2" />
        Nazad na usluge
      </Button>

      <div className="service-details">
        <p className="section-label">{service.category}</p>
        <h1>{service.name}</h1>
        <p className="service-description">{service.description}</p>

        <div className="service-includes">
          <h2>Šta usluga obuhvata?</h2>

          <ul>
            {service.includes.map((item) => (
              <li key={item}>
                <FaCheck className="service-check" />
                {item}
              </li>
            ))}
          </ul>
        </div>

        <p>
          <FaClock className="me-2 text-danger" />
          Trajanje: {service.duration} minuta
        </p>

        <h3 className="mb-4">
          Cena od {service.price.toLocaleString("sr-RS")} RSD
        </h3>

        <Button
          as={Link}
          to={`/zakazivanje?serviceId=${service._id}`}
          variant="danger"
          size="lg"
        >
          Zakaži termin
        </Button>
      </div>
    </Container>
  );
}

export default ServiceDetailsPage;