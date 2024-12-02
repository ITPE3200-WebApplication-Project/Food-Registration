import React from "react";
import { useSearchParams, useNavigate } from "react-router";

const MessageDialog: React.FC = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const message = searchParams.get("message");
  const messageType = searchParams.get("messageType") || "info";

  if (!message) return null;

  const alertClass = (() => {
    switch (messageType.toLowerCase()) {
      case "danger":
        return "alert-danger";
      case "warning":
        return "alert-warning";
      case "success":
        return "alert-success";
      default:
        return "alert-info";
    }
  })();

  const handleClose = () => {
    // Remove message and messageType from URL params while preserving other params
    const newSearchParams = new URLSearchParams(searchParams);
    newSearchParams.delete("message");
    newSearchParams.delete("messageType");

    // Update URL without these params
    navigate({ search: newSearchParams.toString() });
  };

  return (
    <div
      className={`alert ${alertClass} alert-dismissible fade show`}
      role="alert"
    >
      {message}
      <button
        type="button"
        className="btn-close"
        data-bs-dismiss="alert"
        aria-label="Close"
        onClick={handleClose}
      />
    </div>
  );
};

export default MessageDialog;
