import LoginLayout from "@/Layouts/LoginLayout";
import LoginComponent from "@/components/Login/LoginComponent";
import { useAuth } from '@/contexts/AuthContext';

const LoginPage = () => {
  const { login } = useAuth();

  return (
    <LoginComponent login={login} />
  );
};

LoginPage.getLayout = function getLayout(page) {
  return (
    <LoginLayout>
      {page}
    </LoginLayout>
  );
};

export default LoginPage;