import { useLayoutEffect, useRef, useState } from "react";
import {
  AppBar,
  Box,
  Container,
  IconButton,
  Menu,
  MenuItem,
  Toolbar,
  Typography,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import { NavLink } from "react-router-dom";
import MenuItemLink from "./MenuItemLink";
import logo from "/lotto-checker.webp";

const navigation = [
  { to: "/super-lotto", label: "Super Lotto" },
  { to: "/powerBI-Report", label: "Mega Ball Occurrence (Power BI)" },
  { to: "/tableau-Report", label: "Draw Ball Occurrence (Tableau)" },
  { to: "/about", label: "About" },
];

export default function NavBar() {
  const toolbarRef = useRef<HTMLDivElement | null>(null);
  const fullHeaderRef = useRef<HTMLDivElement | null>(null);
  // Begin compact so the full navigation never briefly appears clipped.
  const [compact, setCompact] = useState(true);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  useLayoutEffect(() => {
    const toolbar = toolbarRef.current;
    const fullHeader = fullHeaderRef.current;
    if (!toolbar || !fullHeader) return;

    const updateLayout = () => {
      // The measurement contains the exact desktop logo and navigation,
      // including their spacing. It is never displayed to the user.
      const requiredWidth = fullHeader.getBoundingClientRect().width;
      const availableWidth = toolbar.clientWidth;
      setCompact(requiredWidth + 2 > availableWidth);
    };

    updateLayout();
    const observer = new ResizeObserver(updateLayout);
    observer.observe(toolbar);
    observer.observe(fullHeader);
    window.addEventListener("resize", updateLayout);
    document.fonts?.ready.then(updateLayout).catch(() => {});

    return () => {
      observer.disconnect();
      window.removeEventListener("resize", updateLayout);
    };
  }, []);

  const brand = (
    <MenuItem
      component={NavLink}
      to="/super-lotto"
      sx={{
        display: "flex",
        alignItems: "center",
        flexShrink: 0,
        minWidth: 0,
        textDecoration: "none",
        color: "inherit",
        px: 1,
      }}
    >
      <Box sx={{ display: "flex", alignItems: "center", whiteSpace: "nowrap" }}>
        <img
          src={logo}
          alt="Lotto Checker logo"
          style={{ width: 36, height: 36, marginRight: 8, flexShrink: 0 }}
        />
        <Box>
          <Typography variant="h5" fontWeight="bold" noWrap>
            Lotto Checker&nbsp;©
          </Typography>
          <Typography sx={{ fontSize: "0.85em", fontWeight: "normal", mt: "-0.25rem" }} noWrap>
            by Gary Sloan
          </Typography>
        </Box>
      </Box>
    </MenuItem>
  );

  const desktopLinks = (
    <Box sx={{ display: "flex", alignItems: "center", gap: 2, flexShrink: 0, whiteSpace: "nowrap" }}>
      {navigation.map(({ to, label }) => (
        <MenuItemLink key={to} to={to}>{label}</MenuItemLink>
      ))}
    </Box>
  );

  return (
    <AppBar position="static">
      <Container maxWidth="xl">
        <Toolbar
          ref={toolbarRef}
          disableGutters
          sx={{ position: "relative", justifyContent: "space-between", flexWrap: "nowrap", minWidth: 0 }}
        >
          {/* Measure the full desktop header even while the compact view is active. */}
          <Box
            ref={fullHeaderRef}
            aria-hidden="true"
            sx={{
              position: "absolute",
              top: 0,
              left: 0,
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              gap: 2,
              width: "max-content",
              visibility: "hidden",
              pointerEvents: "none",
            }}
          >
            {brand}
            {desktopLinks}
          </Box>

          {compact ? (
            <Box sx={{ display: "flex", alignItems: "center", minWidth: 0, width: "100%" }}>
              <IconButton
                color="inherit"
                aria-label="Open navigation menu"
                aria-controls={anchorEl ? "lotto-navigation-menu" : undefined}
                aria-haspopup="true"
                aria-expanded={Boolean(anchorEl)}
                onClick={(event) => setAnchorEl(event.currentTarget)}
                sx={{ flexShrink: 0, mr: 0.5 }}
              >
                <MenuIcon />
              </IconButton>
              {brand}
            </Box>
          ) : (
            <>
              {brand}
              {desktopLinks}
            </>
          )}
        </Toolbar>
      </Container>

      <Menu
        id="lotto-navigation-menu"
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={() => setAnchorEl(null)}
        anchorOrigin={{ vertical: "bottom", horizontal: "left" }}
        transformOrigin={{ vertical: "top", horizontal: "left" }}
      >
        {navigation.map(({ to, label }) => (
          <MenuItem
            key={to}
            component={NavLink}
            to={to}
            onClick={() => setAnchorEl(null)}
            sx={{ whiteSpace: "nowrap" }}
          >
            {label}
          </MenuItem>
        ))}
      </Menu>
    </AppBar>
  );
}
