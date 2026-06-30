import { useState } from "react";
import { Alert, Button, Card, Col, Container, Form, Row } from "react-bootstrap";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

function RegisterPage() {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");

  const { register } = useAuth();
  const navigate = useNavigate();

  async function submitHandler(event) {
    event.preventDefault();

    if (!name || !email || !password || !confirmPassword) {
      setError("Sva polja su obavezna.");
      return;
    }

    if (password.length < 6) {
      setError("Lozinka mora imati najmanje 6 karaktera.");
      return;
    }

    if (password !== confirmPassword) {
      setError("Lozinke se ne poklapaju.");
      return;
    }

    const result = await register(name, email, password);

if (!result.success) {
  setError(result.message);
  return;
}

navigate("/");
  }

  return (
    <Container className="py-5">
      <Row className="justify-content-center">
        <Col md={7} lg={5}>
          <Card className="auth-card">
            <Card.Body>
              <p className="section-label">Novi korisnik</p>
              <h1 className="mb-4">Registracija</h1>

              {error && <Alert variant="danger">{error}</Alert>}

              <Form onSubmit={submitHandler}>
                <Form.Group className="mb-3" controlId="name">
                  <Form.Label>Ime i prezime</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Unesite ime i prezime"
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                  />
                </Form.Group>

                <Form.Group className="mb-3" controlId="email">
                  <Form.Label>Email adresa</Form.Label>
                  <Form.Control
                    type="email"
                    placeholder="Unesite email"
                    value={email}
                    onChange={(event) => setEmail(event.target.value)}
                  />
                </Form.Group>

                <Form.Group className="mb-3" controlId="password">
                  <Form.Label>Lozinka</Form.Label>
                  <Form.Control
                    type="password"
                    placeholder="Unesite lozinku"
                    value={password}
                    onChange={(event) => setPassword(event.target.value)}
                  />
                </Form.Group>

                <Form.Group className="mb-4" controlId="confirmPassword">
                  <Form.Label>Potvrda lozinke</Form.Label>
                  <Form.Control
                    type="password"
                    placeholder="Ponovite lozinku"
                    value={confirmPassword}
                    onChange={(event) => setConfirmPassword(event.target.value)}
                  />
                </Form.Group>

                <Button type="submit" variant="danger" className="w-100">
                  Registruj se
                </Button>
              </Form>

              <p className="mt-4 mb-0 text-center">
                Već imate nalog? <Link to="/prijava">Prijavite se</Link>
              </p>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
}

export default RegisterPage;