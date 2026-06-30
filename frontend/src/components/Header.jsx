import { Button, Container, Nav, Navbar } from "react-bootstrap";
import { NavLink, useNavigate } from "react-router-dom";
import { FaCar, FaSignInAlt, FaUserPlus } from "react-icons/fa";
import { useAuth } from "../context/AuthContext";

function Header() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function logoutHandler() {
    logout();
    navigate("/");
  }

  return (
    <Navbar bg="dark" variant="dark" expand="lg">
      <Container>
        <Navbar.Brand as={NavLink} to="/">
          <FaCar className="me-2" />
          AutoServis
        </Navbar.Brand>

        <Navbar.Toggle aria-controls="main-navigation" />

        <Navbar.Collapse id="main-navigation">
          <Nav className="ms-auto align-items-lg-center">
            <Nav.Link as={NavLink} to="/">
              Početna
            </Nav.Link>

            <Nav.Link as={NavLink} to="/usluge">
              Usluge
            </Nav.Link>

            {user && user.role === "user" && (
              <>
                <Nav.Link as={NavLink} to="/moja-vozila">
                  Moja vozila
                </Nav.Link>

                <Nav.Link as={NavLink} to="/zakazivanje">
                  Zakazivanje
                </Nav.Link>

                <Nav.Link as={NavLink} to="/moja-zakazivanja">
                  Moja zakazivanja
                </Nav.Link>
              </>
            )}

            {user && user.role === "admin" && (
              <Nav.Link as={NavLink} to="/admin">
                Admin panel
              </Nav.Link>
            )}

            {user && (
  <Nav.Link as={NavLink} to="/profil">
    Moj profil
  </Nav.Link>
)}

            {user ? (
              <>
                <Navbar.Text className="me-lg-3">
                  {user.role === "admin" ? "Administrator" : user.name}
                </Navbar.Text>

                <Button
                  variant="outline-light"
                  size="sm"
                  onClick={logoutHandler}
                >
                  Odjava
                </Button>
              </>
            ) : (
              <>
                <Nav.Link as={NavLink} to="/prijava">
                  <FaSignInAlt className="me-1" />
                  Prijava
                </Nav.Link>

                <Nav.Link as={NavLink} to="/registracija">
                  <FaUserPlus className="me-1" />
                  Registracija
                </Nav.Link>
              </>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default Header;