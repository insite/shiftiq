<%@ Page Language="C#" CodeBehind="Chats.aspx.cs" Inherits="InSite.UI.Portal.Home.Chats" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div class="row position-relative overflow-hidden gx-2 z-1">

              <!-- Contacts list -->
              <div class="col-xl-4">
                <div class="offcanvas-xl offcanvas-start position-absolute position-xl-relative h-100 bg-light rounded-5 border border-xl-0" id="contactsList" data-bs-scroll="true" data-bs-backdrop="false">
                  <div class="rounded-5 overflow-hidden">
                    <div class="card-header w-100 border-0 px-4 pt-4 pb-3">
                      <div class="d-flex d-xl-none justify-content-end mt-n2 mb-2">
                        <button class="btn btn-outline-secondary border-0 px-3 me-n2" type="button" data-bs-dismiss="offcanvas" data-bs-target=" #contactsList">
                          <i class="ai-cross me-2"></i>
                          Close
                        </button>
                      </div>
                      <div class="position-relative">
                        <input class="form-control pe-5" type="text" placeholder="Search by name or email">
                        <i class="ai-search fs-lg text-nav position-absolute top-50 end-0 translate-middle-y me-3"></i>
                      </div>
                    </div>
                    <div class="card-body px-0 pb-4 pb-xl-0 pt-1 simplebar-scrollable-y" data-simplebar="init" style="max-height: 700px;"><div class="simplebar-wrapper" style="margin: -4px 0px 0px;"><div class="simplebar-height-auto-observer-wrapper"><div class="simplebar-height-auto-observer"></div></div><div class="simplebar-mask"><div class="simplebar-offset" style="right: 0px; bottom: 0px;"><div class="simplebar-content-wrapper" tabindex="0" role="region" aria-label="scrollable content" style="height: auto; overflow: hidden scroll;"><div class="simplebar-content" style="padding: 4px 0px 0px;">

                      <!-- Contact -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3" href="#">
                        <div class="position-relative flex-shrink-0 my-1">
                          <img class="rounded-circle" src="Images/06.jpg" width="48" alt="Avatar">
                          <span class="position-absolute bottom-0 end-0 bg-success border border-white rounded-circle me-1" style="width: .625rem; height: .625rem;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Mikaela Collins</div>
                            <p class="text-body fs-sm mb-0">Thank you for ...</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">15:01</span>
                            <span class="badge bg-danger fs-xs lh-1 py-1 px-2">2</span>
                          </div>
                        </div>
                      </a>

                      <!-- Contact -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3" href="#">
                        <div class="position-relative flex-shrink-0 my-1">
                          <img class="rounded-circle" src="Images/08.jpg" width="48" alt="Avatar">
                          <span class="position-absolute bottom-0 end-0 border border-white rounded-circle me-1" style="width: .625rem; height: .625rem; background-color: #d7dde2;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Robert Fox</div>
                            <p class="text-body fs-sm mb-0">Documents have ...</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">13:10</span>
                            <span class="badge bg-danger fs-xs lh-1 py-1 px-2">3</span>
                          </div>
                        </div>
                      </a>

                      <!-- Contact -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3" href="#">
                        <div class="d-flex align-items-center justify-content-center position-relative flex-shrink-0 rounded-circle text-primary fs-lg fw-semibold my-1" style="width: 48px; height: 48px; background-color: rgba(var(--ar-primary-rgb), .15);">
                          FA
                          <span class="position-absolute bottom-0 end-0 border border-white rounded-circle me-1" style="width: .625rem; height: .625rem; background-color: #d7dde2;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Furry Aocklen</div>
                            <p class="text-body fs-sm mb-0">I got it, thanks 👍</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">12:04</span>
                          </div>
                        </div>
                      </a>

                      <!-- Contact (active) -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3 pe-none bg-gray" href="#">
                        <div class="position-relative flex-shrink-0 my-1">
                          <img class="rounded-circle" src="Images/19.jpg" width="48" alt="Avatar">
                          <span class="position-absolute bottom-0 end-0 bg-success border border-white rounded-circle me-1" style="width: .625rem; height: .625rem;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Mark Virgows</div>
                            <p class="text-body fs-sm mb-0">Ac aliquam victoria ...</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">11:36</span>
                          </div>
                        </div>
                      </a>

                      <!-- Contact -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3" href="#">
                        <div class="d-flex align-items-center justify-content-center position-relative flex-shrink-0 rounded-circle text-info fs-lg fw-semibold my-1" style="width: 48px; height: 48px; background-color: rgba(var(--ar-info-rgb), .15);">
                          ND
                          <span class="position-absolute bottom-0 end-0 border border-white rounded-circle me-1" style="width: .625rem; height: .625rem; background-color: #d7dde2;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Nadia Davidson</div>
                            <p class="text-body fs-sm mb-0">Nullam id devices ...</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">20 hr</span>
                          </div>
                        </div>
                      </a>

                      <!-- Contact -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3" href="#">
                        <div class="position-relative flex-shrink-0 my-1">
                          <img class="rounded-circle" src="Images/09.jpg" width="48" alt="Avatar">
                          <span class="position-absolute bottom-0 end-0 border border-white rounded-circle me-1" style="width: .625rem; height: .625rem; background-color: #d7dde2;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Alica Hawkins</div>
                            <p class="text-body fs-sm mb-0">Massa laoreet molit ...</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">23 hr</span>
                          </div>
                        </div>
                      </a>

                      <!-- Contact -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3" href="#">
                        <div class="position-relative flex-shrink-0 my-1">
                          <img class="rounded-circle" src="Images/05.jpg" width="48" alt="Avatar">
                          <span class="position-absolute bottom-0 end-0 bg-success border border-white rounded-circle me-1" style="width: .625rem; height: .625rem;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Natalia Bocous</div>
                            <p class="text-body fs-sm mb-0">Awesome 🚀</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">2 days</span>
                          </div>
                        </div>
                      </a>

                      <!-- Contact -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3" href="#">
                        <div class="position-relative flex-shrink-0 my-1">
                          <img class="rounded-circle" src="Images/20.jpg" width="48" alt="Avatar">
                          <span class="position-absolute bottom-0 end-0 border border-white rounded-circle me-1" style="width: .625rem; height: .625rem; background-color: #d7dde2;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Marry Black</div>
                            <p class="text-body fs-sm mb-0">I have great news!</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">3 days</span>
                          </div>
                        </div>
                      </a>

                      <!-- Contact -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3" href="#">
                        <div class="position-relative flex-shrink-0 my-1">
                          <img class="rounded-circle" src="Images/10.jpg" width="48" alt="Avatar">
                          <span class="position-absolute bottom-0 end-0 bg-success border border-white rounded-circle me-1" style="width: .625rem; height: .625rem;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Jerome Bell</div>
                            <p class="text-body fs-sm mb-0">Ok 👌</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">May 21</span>
                          </div>
                        </div>
                      </a>

                      <!-- Contact -->
                      <a class="d-flex align-items-center text-decoration-none px-4 py-3" href="#">
                        <div class="position-relative flex-shrink-0 my-1">
                          <img class="rounded-circle" src="Images/11.jpg" width="48" alt="Avatar">
                          <span class="position-absolute bottom-0 end-0 border border-white rounded-circle me-1" style="width: .625rem; height: .625rem; background-color: #d7dde2;"></span>
                        </div>
                        <div class="d-flex justify-content-between w-100 ps-2 ms-1 my-1">
                          <div class="me-3">
                            <div class="h6 mb-1">Jerome Bell</div>
                            <p class="text-body fs-sm mb-0">Do you know how ...</p>
                          </div>
                          <div class="text-end">
                            <span class="d-block fs-xs text-body-secondary">2 days</span>
                          </div>
                        </div>
                      </a>
                    </div></div></div></div><div class="simplebar-placeholder" style="width: 317px; height: 884px;"></div></div><div class="simplebar-track simplebar-horizontal" style="visibility: hidden;"><div class="simplebar-scrollbar" style="width: 0px; display: none;"></div></div><div class="simplebar-track simplebar-vertical" style="visibility: visible;"><div class="simplebar-scrollbar" style="height: 554px; transform: translate3d(0px, 0px, 0px); display: block;"></div></div></div>
                  </div>
                </div>
              </div>


              <!-- Chat window -->
              <div class="col-xl-8">
                <div class="card h-100 border-0">

                  <!-- Header -->
                  <div class="navbar card-header w-100 mx-0 px-4">
                    <div class="d-flex align-items-center w-100 px-sm-3">
                      <button class="navbar-toggler d-xl-none me-3 me-sm-4" type="button" data-bs-toggle="offcanvas" data-bs-target="#contactsList" aria-controls="contactsList" aria-label="Toggle contacts list">
                        <span class="navbar-toggler-icon"></span>
                      </button>
                      <div class="position-relative flex-shrink-0">
                        <img class="rounded-circle" src="Images/19.jpg" width="48" alt="Avatar">
                        <span class="position-absolute bottom-0 end-0 bg-success border border-white rounded-circle me-1" style="width: .625rem; height: .625rem;"></span>
                      </div>
                      <div class="h6 ps-2 ms-1 mb-0">Mark Virgows</div>
                      <div class="dropdown ms-auto">
                        <button class="btn btn-icon btn-sm btn-outline-secondary border-0 rounded-circle me-n2" type="button" data-bs-toggle="dropdown" aria-label="Actions">
                          <i class="ai-dots-vertical fs-4 fw-bold"></i>
                        </button>
                        <div class="dropdown-menu dropdown-menu-end my-1">
                          <a class="dropdown-item" href="#">
                            <i class="ai-user fs-base opacity-80 me-2"></i>
                            View profile
                          </a>
                          <a class="dropdown-item" href="#">
                            <i class="ai-bell-off fs-base opacity-80 me-2"></i>
                            Disable notifications
                          </a>
                          <a class="dropdown-item" href="#">
                            <i class="ai-edit fs-base opacity-80 me-2"></i>
                            Edit contact
                          </a>
                          <a class="dropdown-item" href="#">
                            <i class="ai-trash fs-base opacity-80 me-2"></i>
                            Delete contact
                          </a>
                          <a class="dropdown-item" href="#">
                            <i class="ai-logout fs-base opacity-80 me-2"></i>
                            Leave chat
                          </a>
                          <a class="dropdown-item" href="#">
                            <i class="ai-circle-slash fs-base opacity-80 me-2"></i>
                            Block user
                          </a>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Body -->
                  <div class="card-body pb-0 pt-4 simplebar-scrollable-y" data-simplebar="init" style="max-height: 580px;"><div class="simplebar-wrapper" style="margin: -24px -38px 0px;"><div class="simplebar-height-auto-observer-wrapper"><div class="simplebar-height-auto-observer"></div></div><div class="simplebar-mask"><div class="simplebar-offset" style="right: 0px; bottom: 0px;"><div class="simplebar-content-wrapper" tabindex="0" role="region" aria-label="scrollable content" style="height: auto; overflow: hidden scroll;"><div class="simplebar-content" style="padding: 24px 38px 0px;">
                    <div class="text-body-secondary text-center mb-4">May 27, 2023</div>

                    <!-- Message -->
                    <div class="mb-3" style="max-width: 392px;">
                      <div class="d-flex align-items-end mb-2">
                        <div class="flex-shrink-0 pe-2 me-1">
                          <img class="rounded-circle" src="Images/19.jpg" width="48" alt="Avatar">
                        </div>
                        <div class="message-box-start text-dark">Thank you for recommending me as a designer. I have an interview tomorrow!</div>
                      </div>
                      <div class="fs-xs text-body-secondary text-end">11:32 am</div>
                    </div>

                    <!-- Message -->
                    <div class="ms-auto mb-3" style="max-width: 392px;">
                      <div class="d-flex align-items-end mb-2">
                        <div class="message-box-end bg-primary text-white">Oh no thanks! I just know that you are a great specialist!</div>
                        <div class="flex-shrink-0 ps-2 ms-1">
                          <img class="rounded-circle" src="Images/01.jpg" width="48" alt="Avatar">
                        </div>
                      </div>
                      <div class="fs-xs text-body-secondary">
                        <i class="ai-checks text-primary fs-xl mt-n1 me-1"></i>
                        2:18 pm
                      </div>
                    </div>
                    <div class="text-body-secondary text-center my-4">May 29, 2023</div>

                    <!-- Message -->
                    <div class="mb-3" style="max-width: 392px;">
                      <div class="d-flex align-items-end mb-2">
                        <div class="flex-shrink-0 pe-2 me-1">
                          <img class="rounded-circle" src="Images/19.jpg" width="48" alt="Avatar">
                        </div>
                        <div class="w-100">
                          <div class="message-box-start text-dark mb-2">I have great news, I've been hired! 🚀</div>
                          <div class="message-box-start text-dark">Thanks again!</div>
                        </div>
                      </div>
                      <div class="fs-xs text-body-secondary text-end">4:04 am</div>
                    </div>

                    <!-- Message -->
                    <div class="ms-auto mb-3" style="max-width: 392px;">
                      <div class="d-flex align-items-end mb-2">
                        <div class="message-box-end bg-primary text-white">You are welcome!</div>
                        <div class="flex-shrink-0 ps-2 ms-1">
                          <img class="rounded-circle" src="Images/01.jpg" width="48" alt="Avatar">
                        </div>
                      </div>
                      <div class="fs-xs text-body-secondary">
                        <i class="ai-check text-primary fs-xl mt-n1 me-1"></i>
                        2:18 pm
                      </div>
                    </div>
                    <div class="text-body-secondary text-center my-4">Today</div>

                    <!-- Message -->
                    <div class="mb-3" style="max-width: 392px;">
                      <div class="d-flex align-items-end mb-2">
                        <div class="flex-shrink-0 pe-2 me-1">
                          <img class="rounded-circle" src="Images/19.jpg" width="48" alt="Avatar">
                        </div>
                        <div class="message-box-start text-dark">I'm so happy to be part of the team and work with you on this new exciting project. Can't thank you enough 😊</div>
                      </div>
                      <div class="fs-xs text-body-secondary text-end">10:17 am</div>
                    </div>
                  </div></div></div></div><div class="simplebar-placeholder" style="width: 641px; height: 834px;"></div></div><div class="simplebar-track simplebar-horizontal" style="visibility: hidden;"><div class="simplebar-scrollbar" style="width: 0px; display: none;"></div></div><div class="simplebar-track simplebar-vertical" style="visibility: visible;"><div class="simplebar-scrollbar" style="height: 403px; transform: translate3d(0px, 0px, 0px); display: block;"></div></div></div>

                  <!-- Footer (Textarea) -->
                  <div class="card-footer w-100 border-0 mx-0 px-4">
                    <div class="d-flex align-items-end border rounded-3 pb-3 pe-3 mx-sm-3">
                      <textarea class="form-control border-0" rows="3" placeholder="Type a message" style="resize: none;"></textarea>
                      <div class="nav flex-nowrap align-items-center">
                        <a class="nav-link text-body-secondary p-1 me-1" href="#" aria-label="Attach">
                          <i class="ai-paperclip fs-xl"></i>
                        </a>
                        <a class="nav-link text-body-secondary p-1" href="#" aria-label="Emoji">
                          <i class="ai-emoji-smile fs-xl"></i>
                        </a>
                        <button class="btn btn-sm btn-secondary ms-3" type="button">Send</button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

</asp:Content>