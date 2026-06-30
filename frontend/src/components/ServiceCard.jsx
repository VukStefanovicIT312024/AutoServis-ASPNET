import { Button, Card } from "react-bootstrap";
import { FaClock, FaWrench } from "react-icons/fa";
import { Link } from "react-router-dom";

function ServiceCard({ service }) {
  return (
    <Card className="service-card h-100">
      <Card.Body className="d-flex flex-column">
        <div className="service-icon">
          <FaWrench />
        </div>

        <Card.Subtitle className="mb-2 text-danger">
          {service.category}
        </Card.Subtitle>

        <Card.Title>{service.name}</Card.Title>
        <Card.Text>{service.description}</Card.Text>

        <div className="mt-auto">
          <p className="mb-2">
            <FaClock className="me-2" />
            {service.duration} minuta
          </p>

          <strong className="d-block mb-3">
            Od {service.price.toLocaleString("sr-RS")} RSD
          </strong>

          <Button
            as={Link}
            to={`/usluge/${service._id}`}
            variant="outline-danger"
          >
            Detaljnije
          </Button>
        </div>
      </Card.Body>
    </Card>
  );
}

export default ServiceCard;