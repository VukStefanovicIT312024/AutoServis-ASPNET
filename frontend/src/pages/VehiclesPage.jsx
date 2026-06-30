import { useCallback, useEffect, useState } from "react";
import { Alert, Button, Col, Container, Form, Row, Spinner } from "react-bootstrap";
import { FaEdit, FaTrash } from "react-icons/fa";
import { Link } from "react-router-dom";
import { API_URL } from "../config";
import { useAuth } from "../context/AuthContext";

function VehiclesPage() {
  const { user } = useAuth();

  const [vehicles, setVehicles] = useState([]);
  const [brand, setBrand] = useState("");
  const [model, setModel] = useState("");
  const [year, setYear] = useState("");
  const [plateNumber, setPlateNumber] = useState("");
  const [editingId, setEditingId] = useState(null);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  const fetchVehicles = useCallback(async function () {
    try {
      setLoading(true);

      const response = await fetch(`${API_URL}/api/vehicles`, {
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.message || "Greška pri učitavanju vozila.");
      }

      setVehicles(data);
      setError("");
    } catch (error) {
      setError(error.message);
    } finally {
      setLoading(false);
    }
  }, [user]);

  useEffect(() => {
  if (user) {
    fetchVehicles();
  }
}, [user, fetchVehicles]);

  async function submitHandler(event) {
    event.preventDefault();

    const vehicleData = {
      brand,
      model,
      year: Number(year),
      plateNumber,
    };

    const url = editingId
      ? `${API_URL}/api/vehicles/${editingId}`
      : `${API_URL}/api/vehicles`;

    const method = editingId ? "PUT" : "POST";

    try {
      const response = await fetch(url, {
        method,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
        body: JSON.stringify(vehicleData),
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.message || "Greška pri čuvanju vozila.");
      }

      setBrand("");
      setModel("");
      setYear("");
      setPlateNumber("");
      setEditingId(null);
      setMessage(editingId ? "Vozilo je izmenjeno." : "Vozilo je dodato.");
      setError("");

      fetchVehicles();
    } catch (error) {
      setError(error.message);
      setMessage("");
    }
  }

  function editVehicle(vehicle) {
    setBrand(vehicle.brand);
    setModel(vehicle.model);
    setYear(vehicle.year);
    setPlateNumber(vehicle.plateNumber);
    setEditingId(vehicle._id);
    setMessage("");
    setError("");
  }

  async function deleteVehicle(id) {
    try {
      const response = await fetch(`${API_URL}/api/vehicles/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${user.token}`,
        },
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.message || "Greška pri brisanju vozila.");
      }

      setVehicles(vehicles.filter((vehicle) => vehicle._id !== id));
      setMessage("Vozilo je obrisano.");
      setError("");
    } catch (error) {
      setError(error.message);
      setMessage("");
    }
  }

  if (!user) {
    return (
      <Container className="py-5">
        <Alert variant="warning">
          Morate biti prijavljeni da biste upravljali vozilima.
        </Alert>

        <Button as={Link} to="/prijava" variant="danger">
          Idi na prijavu
        </Button>
      </Container>
    );
  }

  return (
    <Container className="py-5">
      <div className="page-heading">
        <p className="section-label">Korisnički deo</p>
        <h1>Moja vozila</h1>
        <p>Dodajte vozila za koja želite da zakazujete servisne usluge.</p>
      </div>

      {error && <Alert variant="danger">{error}</Alert>}
      {message && <Alert variant="success">{message}</Alert>}

      <Row className="g-4">
        <Col lg={5}>
          <div className="form-panel">
            <h2>{editingId ? "Izmena vozila" : "Dodaj vozilo"}</h2>

            <Form onSubmit={submitHandler}>
              <Form.Group className="mb-3">
                <Form.Label>Marka</Form.Label>
                <Form.Control
                  value={brand}
                  onChange={(event) => setBrand(event.target.value)}
                  placeholder="npr. Volkswagen"
                  required
                />
              </Form.Group>

              <Form.Group className="mb-3">
                <Form.Label>Model</Form.Label>
                <Form.Control
                  value={model}
                  onChange={(event) => setModel(event.target.value)}
                  placeholder="npr. Golf 7"
                  required
                />
              </Form.Group>

              <Form.Group className="mb-3">
                <Form.Label>Godina proizvodnje</Form.Label>
                <Form.Control
                  type="number"
                  value={year}
                  onChange={(event) => setYear(event.target.value)}
                  placeholder="npr. 2018"
                  required
                />
              </Form.Group>

              <Form.Group className="mb-4">
                <Form.Label>Registracija</Form.Label>
                <Form.Control
                  value={plateNumber}
                  onChange={(event) => setPlateNumber(event.target.value)}
                  placeholder="npr. NS-123-AA"
                  required
                />
              </Form.Group>

              <Button type="submit" variant="danger">
                {editingId ? "Sačuvaj izmene" : "Dodaj vozilo"}
              </Button>
            </Form>
          </div>
        </Col>

        <Col lg={7}>
          {loading ? (
            <div className="text-center py-5">
              <Spinner animation="border" variant="danger" />
              <p className="mt-3">Učitavanje vozila...</p>
            </div>
          ) : (
            <div className="vehicle-list">
              {vehicles.map((vehicle) => (
                <div className="vehicle-item" key={vehicle._id}>
                  <div>
                    <h3>
                      {vehicle.brand} {vehicle.model}
                    </h3>
                    <p>
                      {vehicle.year}. godište | {vehicle.plateNumber}
                    </p>
                  </div>

                  <div className="vehicle-actions">
                    <Button
                      variant="outline-dark"
                      size="sm"
                      onClick={() => editVehicle(vehicle)}
                    >
                      <FaEdit />
                    </Button>

                    <Button
                      variant="outline-danger"
                      size="sm"
                      onClick={() => deleteVehicle(vehicle._id)}
                    >
                      <FaTrash />
                    </Button>
                  </div>
                </div>
              ))}

              {vehicles.length === 0 && (
                <p className="text-muted">Još uvek nemate dodata vozila.</p>
              )}
            </div>
          )}
        </Col>
      </Row>
    </Container>
  );
}

export default VehiclesPage;