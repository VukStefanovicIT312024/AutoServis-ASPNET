import { useState } from "react";
import { Alert, Button, Card, Col, Container, Form, Row } from "react-bootstrap";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

function ProfilePage() {
  const { user, updateProfile } = useAuth();

  const [name, setName] = useState(user?.name || "");
  const [phone, setPhone] = useState(user?.phone || "");
  const [city, setCity] = useState(user?.city || "");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  async function submitHandler(event) {
    event.preventDefault();

    if (!name) {
      setError("Ime i prezime su obavezni.");
      setMessage("");
      return;
    }

    const result = await updateProfile({
  name,
  phone,
  city,
});

if (!result.success) {
  setError(result.message);
  setMessage("");
  return;
}

setError("");
setMessage("Podaci profila su uspešno sačuvani.");
  }

  if (!user) {
    return (
      <Container className="py-5">
        <Card className="info-card">
          <Card.Body>
            <p className="section-label">Zaštićena stranica</p>
            <h1>Korisnički profil</h1>
            <p>Da biste pregledali profil, potrebno je da se prvo prijavite.</p>

            <Button as={Link} to="/prijava" variant="danger">
              Prijava
            </Button>
          </Card.Body>
        </Card>
      </Container>
    );
  }

  return (
    <Container className="py-5">
      <div className="page-heading">
        <p className="section-label">Korisnički nalog</p>
        <h1>Moj profil</h1>
        <p>Pregled i izmena osnovnih podataka korisničkog naloga.</p>
      </div>

      <Row className="g-4">
        <Col lg={5}>
          <Card className="profile-card">
            <Card.Body>
              <h2>Podaci naloga</h2>

              <div className="profile-info">
                <p>
                  <strong>Ime:</strong> {user.name}
                </p>
                <p>
                  <strong>Email:</strong> {user.email}
                </p>
                <p>
                  <strong>Uloga:</strong>{" "}
                  {user.role === "admin" ? "Administrator" : "Korisnik"}
                </p>
                <p>
                  <strong>Telefon:</strong> {user.phone || "Nije unet"}
                </p>
                <p>
                  <strong>Grad:</strong> {user.city || "Nije unet"}
                </p>
              </div>
            </Card.Body>
          </Card>
        </Col>

        <Col lg={7}>
          <Card className="form-card">
            <Card.Body>
              <h2>Izmena profila</h2>

              {error && <Alert variant="danger">{error}</Alert>}
              {message && <Alert variant="success">{message}</Alert>}

              <Form onSubmit={submitHandler}>
                <Form.Group className="mb-3">
                  <Form.Label>Ime i prezime</Form.Label>
                  <Form.Control
                    type="text"
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                  />
                </Form.Group>

                <Form.Group className="mb-3">
                  <Form.Label>Email adresa</Form.Label>
                  <Form.Control type="email" value={user.email} disabled />
                  <Form.Text>
                    Email adresa se u ovoj fazi ne menja zbog povezivanja sa
                    vozilima i zakazivanjima.
                  </Form.Text>
                </Form.Group>

                <Form.Group className="mb-3">
                  <Form.Label>Telefon</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Na primer: 0601234567"
                    value={phone}
                    onChange={(event) => setPhone(event.target.value)}
                  />
                </Form.Group>

                <Form.Group className="mb-4">
                  <Form.Label>Grad</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Na primer: Novi Sad"
                    value={city}
                    onChange={(event) => setCity(event.target.value)}
                  />
                </Form.Group>

                <Button type="submit" variant="danger">
                  Sačuvaj izmene
                </Button>
              </Form>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}

export default ProfilePage;